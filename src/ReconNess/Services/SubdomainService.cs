using Microsoft.EntityFrameworkCore;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ISubdomainService"/> 
    /// </summary>
    public class SubdomainService : Service<Subdomain>, IService<Subdomain>, ISubdomainService
    {
        private readonly ILabelService labelService;
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISubdomainService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="labelService"><see cref="ILabelService"/></param>
        /// <param name="notificationService"><see cref="INotificationService"/></param>
        public SubdomainService(
            IUnitOfWork unitOfWork,
            ILabelService labelService,
            INotificationService notificationService)
            : base(unitOfWork)
        {
            this.labelService = labelService;
            this.notificationService = notificationService;
        }

        /// <summary>
        /// <see cref="ISubdomainService.GetSubdomainsByTargetAsync(RootDomain, CancellationToken)"/>
        /// </summary>
        public async Task<List<Subdomain>> GetSubdomainsByTargetAsync(RootDomain rootDomain, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(s => s.RootDomain == rootDomain, cancellationToken)
                .Include(t => t.Services)
                .Include(t => t.Notes)
                .Include(t => t.Labels)
                    .ThenInclude(ac => ac.Label)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="ISubdomainService.UpdateSubdomainAsync(AgentRun, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task UpdateSubdomainByAgentRunning(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            await this.UpdateSubdomainIpAddress(subdomain, agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainIsAlive(subdomain, agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainHasHttpOpen(subdomain, agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainTakeover(subdomain, agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainDirectory(subdomain, agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainService(subdomain, agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainNote(subdomain, agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainLabel(subdomain, agentRun, scriptOutput, cancellationToken);

            //this.UpdateSubdomainScreenshot(subdomain, scriptOutput);
            this.UpdateSubdomainAgent(subdomain, agentRun.Agent.Name);

            this.UnitOfWork.Repository<Subdomain>().Update(subdomain);
        }

        /// <summary>
        /// <see cref="ISubdomainService.DeleteSubdomainAsync(Subdomain, CancellationToken)"/>
        /// </summary>
        public async Task DeleteSubdomainAsync(Subdomain subdomain, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction(cancellationToken);

                if (subdomain.Notes != null)
                {
                    this.UnitOfWork.Repository<Note>().Delete(subdomain.Notes, cancellationToken);
                }

                this.UnitOfWork.Repository<Service>().DeleteRange(subdomain.Services.ToList(), cancellationToken);
                this.UnitOfWork.Repository<Subdomain>().Delete(subdomain, cancellationToken);

                await this.UnitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                this.UnitOfWork.Rollback(cancellationToken);
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="ISubdomainService.DeleteSubdomains(ICollection{Subdomain}, CancellationToken)"/>
        /// </summary>
        public void DeleteSubdomains(ICollection<Subdomain> subdomains, CancellationToken cancellationToken = default)
        {
            foreach (var subdomain in subdomains)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (subdomain.Notes != null)
                {
                    this.UnitOfWork.Repository<Note>().Delete(subdomain.Notes, cancellationToken);
                }

                this.UnitOfWork.Repository<Service>().DeleteRange(subdomain.Services.ToList(), cancellationToken);
                this.UnitOfWork.Repository<Subdomain>().Delete(subdomain, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="ISubdomainService.UpdateSubdomainAgent(Subdomain, string, CancellationToken)"/>
        /// </summary>
        public async Task UpdateSubdomainAgent(Subdomain subdomain, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.UpdateSubdomainAgent(subdomain, agentName);

            await this.UpdateAsync(subdomain, cancellationToken);           
        }

        /// <summary>
        /// Assign Ip address to the subdomain
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainIpAddress(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (scriptOutput.Ip != null && Helpers.Helpers.ValidateIPv4(scriptOutput.Ip) && subdomain.IpAddress != scriptOutput.Ip)
            {
                subdomain.IpAddress = scriptOutput.Ip;

                var payload = agentRun.Agent.AgentNotification?.IpAddressPayload ?? string.Empty;
                await this.SendNotificationIfActive(agentRun, payload, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{ip}}", scriptOutput.Ip)
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain if it has http port open
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainHasHttpOpen(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (scriptOutput.HasHttpOpen != null && subdomain.HasHttpOpen != scriptOutput.HasHttpOpen.Value)
            {
                subdomain.HasHttpOpen = scriptOutput.HasHttpOpen.Value;

                var payload = agentRun.Agent.AgentNotification?.HasHttpOpenPayload ?? string.Empty;
                await this.SendNotificationIfActive(agentRun, payload, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{httpOpen}}", scriptOutput.HasHttpOpen.Value.ToString())
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain if it can be takeover
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainTakeover(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (scriptOutput.Takeover != null && subdomain.Takeover != scriptOutput.Takeover.Value)
            {
                subdomain.Takeover = scriptOutput.Takeover.Value;

                var payload = agentRun.Agent.AgentNotification?.TakeoverPayload ?? string.Empty;
                await this.SendNotificationIfActive(agentRun, payload, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{takeover}}", scriptOutput.Takeover.Value.ToString())
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain with directory discovery
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainDirectory(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(scriptOutput.HttpDirectory))
            {
                var httpDirectory = scriptOutput.HttpDirectory.TrimEnd('/').TrimEnd();
                if (subdomain.ServiceHttp == null)
                {
                    subdomain.ServiceHttp = new ServiceHttp();
                }

                if (subdomain.ServiceHttp.Directories == null)
                {
                    subdomain.ServiceHttp.Directories = new List<ServiceHttpDirectory>();
                }

                if (subdomain.ServiceHttp.Directories.Any(d => d.Directory == httpDirectory))
                {
                    return;
                }

                var directory = new ServiceHttpDirectory()
                {
                    Directory = httpDirectory,
                    StatusCode = scriptOutput.HttpDirectoryStatusCode,
                    Method = scriptOutput.HttpDirectoryMethod,
                    Size = scriptOutput.HttpDirectorySize
                };

                subdomain.ServiceHttp.Directories.Add(directory);

                var payload = agentRun.Agent.AgentNotification?.DirectoryPayload ?? string.Empty;
                await this.SendNotificationIfActive(agentRun, payload, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{directory}}", httpDirectory)
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain if is a new service with open port
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainService(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(scriptOutput.Service))
            {
                return;
            }

            var service = new Service
            {
                Name = scriptOutput.Service.ToLower(),
                Port = scriptOutput.Port.Value
            };

            if (subdomain.Services == null)
            {
                subdomain.Services = new List<Service>();
            }

            if (!subdomain.Services.Any(s => s.Name == service.Name && s.Port == service.Port))
            {
                subdomain.Services.Add(service);

                var payload = agentRun.Agent.AgentNotification?.ServicePayload ?? string.Empty;
                await this.SendNotificationIfActive(agentRun, payload, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{service}}", service.Name),
                    ("{{port}}", service.Port.ToString())
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain Note
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainNote(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(scriptOutput.Note))
            {
                if (subdomain.Notes == null)
                {
                    subdomain.Notes = new Note();
                }

                subdomain.Notes.Notes = subdomain.Notes.Notes + '\n' + scriptOutput.Note;

                var payload = agentRun.Agent.AgentNotification?.NotePayload ?? string.Empty;
                await this.SendNotificationIfActive(agentRun, payload, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{note}}", scriptOutput.Note)
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain if is Alive
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainIsAlive(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (scriptOutput.IsAlive != null && subdomain.IsAlive != scriptOutput.IsAlive)
            {
                subdomain.IsAlive = scriptOutput.IsAlive.Value;

                var payload = agentRun.Agent.AgentNotification?.IsAlivePayload ?? string.Empty;
                await this.SendNotificationIfActive(agentRun, payload, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{isAlive}}", scriptOutput.IsAlive.Value.ToString())
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain with screenshots
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        private void UpdateSubdomainScreenshot(Subdomain subdomain, ScriptOutput scriptOutput)
        {
            if (string.IsNullOrEmpty(scriptOutput.HttpScreenshotFilePath))
            {
                try
                {
                    var fileBase64 = Convert.ToBase64String(File.ReadAllBytes(scriptOutput.HttpScreenshotFilePath));
                    if (subdomain.ServiceHttp == null)
                    {
                        subdomain.ServiceHttp = new ServiceHttp();
                    }

                    subdomain.ServiceHttp.ScreenshotHttpPNGBase64 = fileBase64;
                }
                catch
                {

                }
            }

            if (string.IsNullOrEmpty(scriptOutput.HttpsScreenshotFilePath))
            {
                try
                {
                    var fileBase64 = Convert.ToBase64String(File.ReadAllBytes(scriptOutput.HttpsScreenshotFilePath));
                    if (subdomain.ServiceHttp == null)
                    {
                        subdomain.ServiceHttp = new ServiceHttp();
                    }

                    subdomain.ServiceHttp.ScreenshotHttpsPNGBase64 = fileBase64;
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Update the subdomain label
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainLabel(Subdomain subdomain, AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(scriptOutput.Label) &&
                !subdomain.Labels.Any(l => scriptOutput.Label.Equals(l.Label.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var label = await this.labelService.GetByCriteriaAsync(l => l.Name.ToLower() == scriptOutput.Label.ToLower(), cancellationToken);
                if (label == null)
                {
                    var random = new Random();
                    label = new Label
                    {
                        Name = scriptOutput.Label,
                        Color = string.Format("#{0:X6}", random.Next(0x1000000))
                    };
                }

                subdomain.Labels.Add(new SubdomainLabel
                {
                    Label = label,
                    SubdomainId = subdomain.Id
                });
            }
        }

        /// <summary>
        /// Update the subdomain agent property with the agent name if was updated before
        /// </summary>
        /// <param name="agentRun">The agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        private void UpdateSubdomainAgent(Subdomain subdomain, string agentName)
        {
            if (string.IsNullOrWhiteSpace(subdomain.FromAgents))
            {
                subdomain.FromAgents = agentName;
            }
            else if (!subdomain.FromAgents.Contains(agentName))
            {
                subdomain.FromAgents = string.Join(", ", subdomain.FromAgents, agentName);
            }
        }

        /// <summary>
        /// Send notifications if it is actived
        /// </summary>
        /// <param name="agentRun">The Agent running</param>
        /// <param name="payload">The payload</param>
        /// <param name="replaces">The replacement values</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        private async Task SendNotificationIfActive(AgentRun agentRun, string payload, (string, string)[] replaces, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(payload) && agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound)
            {
                foreach (var replace in replaces)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    payload = payload.Replace(replace.Item1, replace.Item2);
                }

                await this.notificationService.SendAsync(payload, cancellationToken);
            }
        }
    }
}
