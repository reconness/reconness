using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Application.Managers;
using ReconNess.Application.Models;
using ReconNess.Application.Providers;
using ReconNess.Domain.Entities;
using ReconNess.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ReconNess.Application.Services;

/// <summary>
/// This class implement <see cref="IAgentRunnerService"/>
/// </summary>
public class AgentRunnerService : Service<AgentRunner>, IAgentRunnerService, IService<AgentRunner>
{
    private readonly ITargetService targetService;
    private readonly IRootDomainService rootDomainService;
    private readonly ISubdomainService subdomainService;
    private readonly IAgentServerManager agentServerManager;
    private readonly IQueueProvider<AgentRunnerQueue> queueProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentRunnerService" /> class
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
    /// <param name="targetService"><see cref="ITargetService"/></param>
    /// <param name="rootDomainService"><see cref="IRootDomainService"/></param>
    /// <param name="subdomainService"><see cref="ISubdomainService"/></param>
    /// <param name="queueProvider"><see cref="IQueueProvider{T}"/></param>
    /// <param name="agentServerManager"><see cref="IAgentServerManager"/></param>
    public AgentRunnerService(IUnitOfWork unitOfWork,
        ITargetService targetService,
        IRootDomainService rootDomainService,
        ISubdomainService subdomainService,
        IQueueProvider<AgentRunnerQueue> queueProvider,
        IAgentServerManager agentServerManager) : base(unitOfWork)
    {
        this.targetService = targetService;
        this.rootDomainService = rootDomainService;
        this.subdomainService = subdomainService;
        this.queueProvider = queueProvider;
        this.agentServerManager = agentServerManager;
    }

    /// <inheritdoc/>
    public async Task<string> RunAgentAsync(AgentRunnerInfo agentRunnerInfo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var agentRunnerType = GetAgentRunnerType(agentRunnerInfo);
        var channel = await GetChannelAsync(agentRunnerInfo, cancellationToken);
        if (agentRunnerType.StartsWith("Current"))
        {
            await EnqueueAgentRunnerCurrentConceptAsync(channel, agentRunnerInfo, cancellationToken);
        }
        else
        {
            await EnqueueAgentRunnerForEachSubConceptAsync(channel, agentRunnerInfo, agentRunnerType, cancellationToken);
        }

        return channel;
    }

    /// <inheritdoc/>
    public async Task StopAgentAsync(string channel, CancellationToken cancellationToken = default)
    {
        var agentRunner = await GetByCriteriaAsync(a => a.Channel == channel, cancellationToken);
        if (agentRunner != null && (agentRunner.Stage == AgentRunnerStage.ENQUEUE || agentRunner.Stage == AgentRunnerStage.RUNNING))
        {
            agentRunner.Stage = AgentRunnerStage.STOPPED;
            await UpdateAsync(agentRunner, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> RunningAgentsAsync(CancellationToken cancellationToken = default) => 
        await UnitOfWork.Repository<IAgentRunnerRepository, AgentRunner>().RunningAgentsAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> TerminalOutputAsync(string channel, CancellationToken cancellationToken = default)
    {
        var result = new List<string>();

        var agentRunner = await UnitOfWork.Repository<IAgentRunnerRepository, AgentRunner>().GetAgentRunnerAsync(channel, cancellationToken);
        if (agentRunner != null)
        {
            foreach (var command in agentRunner.Commands.OrderBy(c => c.CreatedAt))
            {
                result.AddRange(command.Outputs.OrderBy(o => o.CreatedAt).Select(o => o.Output));
            }
        }

        return result;
    }

    /// <summary>
    /// Enqueue current concept [target, rootdomain, subdomain]
    /// </summary>
    /// <param name="channel">The channel</param>
    /// <param name="agentRunnerInfo">The agent run parameters</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A task</returns>
    private async Task EnqueueAgentRunnerCurrentConceptAsync(string channel, AgentRunnerInfo agentRunnerInfo, CancellationToken cancellationToken)
    {
        await AddAsync(new AgentRunner
        {
            Channel = channel,
            Stage = AgentRunnerStage.ENQUEUE,
            AllowSkip = false,
            Total = 1,
            ActivateNotification = agentRunnerInfo.ActivateNotification,
            Agent = agentRunnerInfo.Agent
        }, cancellationToken);

        var command = GetCommand(agentRunnerInfo);
        var agentRunnerQueue = new AgentRunnerQueue
        {
            Channel = channel,
            Command = command,
            Number = 1
        };

        await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
    }

    /// <summary>
    /// Enqueue for each sub concept [target, rootdomain, subdomain]
    /// </summary>
    /// <param name="channel">The channel</param>
    /// <param name="agentRunnerInfo">The agent run parameters</param>
    /// <param name="agentRunnerType">The sublevel <see cref="AgentRunnerTypes"/></param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Task</returns>
    private async Task EnqueueAgentRunnerForEachSubConceptAsync(string channel, AgentRunnerInfo agentRunnerInfo, string agentRunnerType, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (AgentRunnerTypes.ALL_TARGETS.Equals(agentRunnerType))
        {
            await EnqueueRunAgenthForEachTargetsAsync(agentRunnerInfo, channel, cancellationToken);
        }
        else if (AgentRunnerTypes.ALL_ROOTDOMAINS.Equals(agentRunnerType))
        {
            await EnqueueRunAgentForEachRootDomainsAsync(agentRunnerInfo, channel, cancellationToken);
        }
        else if (AgentRunnerTypes.ALL_SUBDOMAINS.Equals(agentRunnerType))
        {
            await EnqueueRunAgentForEachSubdomainsAsync(agentRunnerInfo, channel, cancellationToken);
        }
    }

    /// <summary>
    /// Run bash for each Target
    /// </summary>
    /// <param name="agentRunnerInfo">The agent run parameters</param>
    /// <param name="channel">The channel to send the menssage</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Task</returns>
    private async Task EnqueueRunAgenthForEachTargetsAsync(AgentRunnerInfo agentRunnerInfo, string channel, CancellationToken cancellationToken)
    {
        var targets = await targetService.GetAllAsync(cancellationToken);
        var stage = targets.Any() ? AgentRunnerStage.ENQUEUE : AgentRunnerStage.SUCCESS;

        await AddAsync(new AgentRunner
        {
            Channel = channel,
            Stage = stage,
            AllowSkip = true,
            Total = targets.Count,
            ActivateNotification = agentRunnerInfo.ActivateNotification,
            Agent = agentRunnerInfo.Agent
        }, cancellationToken);

        var count = 1;
        foreach (var target in targets)
        {
            agentRunnerInfo.Target = target;
            var command = GetCommand(agentRunnerInfo);

            var agentRunnerQueue = new AgentRunnerQueue
            {
                Channel = channel,
                Payload = target.Name,
                Command = command,
                Number = count++,
            };

            await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
        }
    }

    /// <summary>
    /// Run bash for each Rootdomain
    /// </summary>
    /// <param name="agentRunnerInfo">The agent run parameters</param>
    /// <param name="channel">The channel to send the menssage</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Task</returns>
    private async Task EnqueueRunAgentForEachRootDomainsAsync(AgentRunnerInfo agentRunnerInfo, string channel, CancellationToken cancellationToken)
    {
        var rootdomains = await rootDomainService.GetAllByCriteriaAsync(r => r.Target == agentRunnerInfo.Target, cancellationToken);
        var stage = rootdomains.Any() ? AgentRunnerStage.ENQUEUE : AgentRunnerStage.SUCCESS;

        await AddAsync(new AgentRunner
        {
            Channel = channel,
            Stage = stage,
            AllowSkip = true,
            Total = rootdomains.Count,
            ActivateNotification = agentRunnerInfo.ActivateNotification,
            Agent = agentRunnerInfo.Agent
        }, cancellationToken);

        var count = 1;
        foreach (var rootdomain in rootdomains)
        {
            agentRunnerInfo.RootDomain = rootdomain;
            var command = GetCommand(agentRunnerInfo);

            var agentRunnerQueue = new AgentRunnerQueue
            {
                Channel = channel,
                Payload = rootdomain.Name,
                Command = command,
                Number = count++
            };

            await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
        }
    }

    /// <summary>
    /// Run bash for each Subdomain
    /// </summary>
    /// <param name="agentRunnerInfo">The agent run parameters</param>
    /// <param name="channel">The channel to send the menssage</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A Task</returns>
    private async Task EnqueueRunAgentForEachSubdomainsAsync(AgentRunnerInfo agentRunnerInfo, string channel, CancellationToken cancellationToken)
    {
        var subdomains = await subdomainService.GetSubdomainsAsync(s => s.RootDomain == agentRunnerInfo.RootDomain, cancellationToken);
        var stage = subdomains.Any() ? AgentRunnerStage.ENQUEUE : AgentRunnerStage.SUCCESS;

        await AddAsync(new AgentRunner
        {
            Channel = channel,
            Stage = stage,
            AllowSkip = true,
            Total = subdomains.Count,
            ActivateNotification = agentRunnerInfo.ActivateNotification,
            Agent = agentRunnerInfo.Agent
        }, cancellationToken);

        var count = 1;
        foreach (var subdomain in subdomains)
        {
            agentRunnerInfo.Subdomain = subdomain;
            var command = GetCommand(agentRunnerInfo);

            var agentRunnerQueue = new AgentRunnerQueue
            {
                Channel = channel,
                Payload = subdomain.Name,
                Command = command,
                Number = count++
            };

            await EnqueueRunAgentAsync(agentRunnerQueue, cancellationToken);
        }
    }

    /// <summary>
    /// Run bash
    /// </summary>
    /// <param name="agentRunnerQueue">The agent runner queue information</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>A task</returns>
    private async Task EnqueueRunAgentAsync(AgentRunnerQueue agentRunnerQueue, CancellationToken cancellationToken = default)
    {
        agentRunnerQueue.ServerNumber = await agentServerManager.GetAvailableServerAsync(agentRunnerQueue.Channel, 60, cancellationToken);

        queueProvider.Enqueue(agentRunnerQueue);
    }

    /// <summary>
    /// Obtain the channel.
    /// 
    /// Ex 
    /// #20220319.1_nmap_yahoo_yahoo.com_www.yahoo.com
    /// #20220318.2_nmap_yahoo_yahoo.com_www.yahoo.com
    /// #20220318.1_nmap_yahoo_yahoo.com_www.yahoo.com
    /// 
    /// </summary>
    /// <param name="agentRunnerInfo">The agent runner</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The agent runner channel</returns>
    private async Task<string> GetChannelAsync(AgentRunnerInfo agentRunnerInfo, CancellationToken cancellationToken = default)
    {
        string channel = string.Empty;

        if (agentRunnerInfo.RootDomain == null)
        {
            channel = $"{agentRunnerInfo.Agent.Name}_{agentRunnerInfo.Target.Name}_all";
        }
        else if (agentRunnerInfo.Subdomain == null)
        {
            channel = $"{agentRunnerInfo.Agent.Name}_{agentRunnerInfo.Target.Name}_{agentRunnerInfo.RootDomain.Name}_all";
        }
        else
        {
            channel = $"{agentRunnerInfo.Agent.Name}_{agentRunnerInfo.Target.Name}_{agentRunnerInfo.RootDomain.Name}_{agentRunnerInfo.Subdomain.Name}";
        }

        var prefix = $"#{DateTime.Now:yyyyMMdd}";
        var count = await UnitOfWork.Repository<IAgentRunnerRepository, AgentRunner>()
            .GetChannelCountAsync(r => r.Channel.EndsWith(channel) && r.Channel.StartsWith(prefix), cancellationToken);            

        // Ex. #20220319.1_nmap_yahoo_yahho.com_www.yahoo.com
        channel = $"{prefix}.{++count}_{channel}";

        return channel;
    }

    /// <summary>
    /// Obtain the command to run on bash
    /// </summary>
    /// <param name="agentRunnerInfo">The agent</param>
    /// <returns>The command to run on bash</returns>
    private static string GetCommand(AgentRunnerInfo agentRunnerInfo)
    {
        var command = agentRunnerInfo.Command;
        if (string.IsNullOrWhiteSpace(command))
        {
            command = agentRunnerInfo.Agent.Command;
        }

        var envUserName = Environment.GetEnvironmentVariable("ReconnessUserName") ??
                          Environment.GetEnvironmentVariable("ReconnessUserName", EnvironmentVariableTarget.User);

        var envPassword = Environment.GetEnvironmentVariable("ReconnessPassword") ??
                          Environment.GetEnvironmentVariable("ReconnessPassword", EnvironmentVariableTarget.User);

        return command
            .Replace("{{target}}", agentRunnerInfo.Target.Name)
            .Replace("{{rootDomain}}", agentRunnerInfo.RootDomain.Name)
            .Replace("{{rootdomain}}", agentRunnerInfo.RootDomain.Name)
            .Replace("{{domain}}", agentRunnerInfo.Subdomain == null ? agentRunnerInfo.RootDomain.Name : agentRunnerInfo.Subdomain.Name)
            .Replace("{{subdomain}}", agentRunnerInfo.Subdomain == null ? agentRunnerInfo.RootDomain.Name : agentRunnerInfo.Subdomain.Name)
            .Replace("{{userName}}", envUserName)
            .Replace("{{password}}", envPassword)
            .Replace("\"", "\\\"");
    }

    /// <summary>
    /// If we need to run the Agent in each subdomain
    /// </summary>
    /// <param name="agentRunnerInfo">The agent run parameters</param>
    /// <returns>If we need to run the Agent in each subdomain</returns>
    /// <exception cref="ArgumentException">If the Agent does not have a valid Type</exception>
    private static string GetAgentRunnerType(AgentRunnerInfo agentRunnerInfo)
    {
        var type = agentRunnerInfo.Agent.AgentType;
        return type switch
        {
            AgentTypes.TARGET => agentRunnerInfo.Target == null ? AgentRunnerTypes.ALL_TARGETS : AgentRunnerTypes.CURRENT_TARGET,
            AgentTypes.ROOTDOMAIN => agentRunnerInfo.RootDomain == null ? AgentRunnerTypes.ALL_ROOTDOMAINS : AgentRunnerTypes.CURRENT_ROOTDOMAIN,
            AgentTypes.SUBDOMAIN => agentRunnerInfo.Subdomain == null ? AgentRunnerTypes.ALL_SUBDOMAINS : AgentRunnerTypes.CURRENT_SUBDOMAIN,
            _ => throw new ArgumentException("The Agent does not have a valid Type")
        };
    }
}
