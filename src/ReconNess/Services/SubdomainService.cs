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
        public async Task UpdateSubdomainByAgent(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            await this.UpdateSubdomainIpAddress(agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainIsAlive(agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainHasHttpOpen(agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainTakeover(agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainDirectory(agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainService(agentRun, scriptOutput, cancellationToken);
            await this.UpdateSubdomainNote(agentRun, scriptOutput, cancellationToken);

            await this.UpdateSubdomainLabel(agentRun, scriptOutput, cancellationToken);
            this.UpdateSubdomainAgent(agentRun, cancellationToken);
            this.UpdateSubdomainScreenshot(agentRun, scriptOutput, cancellationToken);

            this.UnitOfWork.Repository<Subdomain>().Update(agentRun.Subdomain);
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
        /// Assign Ip address to the subdomain
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainIpAddress(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (scriptOutput.Ip != null && Helpers.Helpers.ValidateIPv4(scriptOutput.Ip) && agentRun.Subdomain.IpAddress != scriptOutput.Ip)
            {
                agentRun.Subdomain.IpAddress = scriptOutput.Ip;

                if (agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound && agentRun.Agent.AgentNotification != null && !string.IsNullOrEmpty(agentRun.Agent.AgentNotification.IpAddressPayload))
                {
                    var payload = agentRun.Agent.AgentNotification.IpAddressPayload.Replace("{{domain}}", agentRun.Subdomain.Name).Replace("{{ip}}", scriptOutput.Ip);
                    await this.notificationService.SendAsync(payload, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain if it has http port open
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainHasHttpOpen(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (scriptOutput.HasHttpOpen != null && agentRun.Subdomain.HasHttpOpen != scriptOutput.HasHttpOpen.Value)
            {
                agentRun.Subdomain.HasHttpOpen = scriptOutput.HasHttpOpen.Value;

                if (agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound && agentRun.Agent.AgentNotification != null && !string.IsNullOrEmpty(agentRun.Agent.AgentNotification.HasHttpOpenPayload))
                {
                    var payload = agentRun.Agent.AgentNotification.HasHttpOpenPayload.Replace("{{domain}}", agentRun.Subdomain.Name).Replace("{{httpOpen}}", scriptOutput.HasHttpOpen.Value.ToString());
                    await this.notificationService.SendAsync(payload, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain if it can be takeover
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainTakeover(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (scriptOutput.Takeover != null && agentRun.Subdomain.Takeover != scriptOutput.Takeover.Value)
            {
                agentRun.Subdomain.Takeover = scriptOutput.Takeover.Value;

                if (agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound && agentRun.Agent.AgentNotification != null && !string.IsNullOrEmpty(agentRun.Agent.AgentNotification.TakeoverPayload))
                {
                    var payload = agentRun.Agent.AgentNotification.TakeoverPayload.Replace("{{domain}}", agentRun.Subdomain.Name).Replace("{{takeover}}", scriptOutput.Takeover.Value.ToString());
                    await this.notificationService.SendAsync(payload, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain with screenshots
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        private void UpdateSubdomainScreenshot(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(scriptOutput.HttpScreenshotFilePath))
            {
                try
                {
                    var fileBase64 = Convert.ToBase64String(File.ReadAllBytes(scriptOutput.HttpScreenshotFilePath));
                    if (agentRun.Subdomain.ServiceHttp == null)
                    {
                        agentRun.Subdomain.ServiceHttp = new ServiceHttp();
                    }

                    agentRun.Subdomain.ServiceHttp.ScreenshotHttpPNGBase64 = fileBase64;
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
                    if (agentRun.Subdomain.ServiceHttp == null)
                    {
                        agentRun.Subdomain.ServiceHttp = new ServiceHttp();
                    }

                    agentRun.Subdomain.ServiceHttp.ScreenshotHttpsPNGBase64 = fileBase64;
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Update the subdomain with directory discovery
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainDirectory(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(scriptOutput.HttpDirectory))
            {
                var httpDirectory = scriptOutput.HttpDirectory.TrimEnd('/').TrimEnd();
                if (agentRun.Subdomain.ServiceHttp == null)
                {
                    agentRun.Subdomain.ServiceHttp = new ServiceHttp();
                }

                if (agentRun.Subdomain.ServiceHttp.Directories == null)
                {
                    agentRun.Subdomain.ServiceHttp.Directories = new List<ServiceHttpDirectory>();
                }

                if (agentRun.Subdomain.ServiceHttp.Directories.Any(d => d.Directory == httpDirectory))
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

                agentRun.Subdomain.ServiceHttp.Directories.Add(directory);

                if (agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound && agentRun.Agent.AgentNotification != null && !string.IsNullOrEmpty(agentRun.Agent.AgentNotification.DirectoryPayload))
                {
                    var payload = agentRun.Agent.AgentNotification.DirectoryPayload.Replace("{{domain}}", agentRun.Subdomain.Name).Replace("{{directory}}", httpDirectory);
                    await this.notificationService.SendAsync(payload, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain if is a new service with open port
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainService(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
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

            if (agentRun.Subdomain.Services == null)
            {
                agentRun.Subdomain.Services = new List<Service>();
            }

            if (!agentRun.Subdomain.Services.Any(s => s.Name == service.Name && s.Port == service.Port))
            {
                agentRun.Subdomain.Services.Add(service);

                if (agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound && agentRun.Agent.AgentNotification != null && !string.IsNullOrEmpty(agentRun.Agent.AgentNotification.ServicePayload))
                {
                    var payload = agentRun.Agent.AgentNotification.ServicePayload.Replace("{{domain}}", agentRun.Subdomain.Name).Replace("{{service}}", service.Name).Replace("{{port}}", service.Port.ToString());
                    await this.notificationService.SendAsync(payload, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain Note
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainNote(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(scriptOutput.Note))
            {
                if (agentRun.Subdomain.Notes == null)
                {
                    agentRun.Subdomain.Notes = new Note();
                }

                agentRun.Subdomain.Notes.Notes = agentRun.Subdomain.Notes.Notes + '\n' + scriptOutput.Note;
                if (agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound && agentRun.Agent.AgentNotification != null && !string.IsNullOrEmpty(agentRun.Agent.AgentNotification.NotePayload))
                {
                    var payload = agentRun.Agent.AgentNotification.NotePayload.Replace("{{domain}}", agentRun.Subdomain.Name).Replace("{{note}}", scriptOutput.Note);
                    await this.notificationService.SendAsync(payload, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain if is Alive
        /// </summary>
        /// <param name="agentRun">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainIsAlive(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (scriptOutput.IsAlive != null && agentRun.Subdomain.IsAlive != scriptOutput.IsAlive)
            {
                agentRun.Subdomain.IsAlive = scriptOutput.IsAlive.Value;

                if (agentRun.ActivateNotification && agentRun.Agent.NotifyNewFound && agentRun.Agent.AgentNotification != null && !string.IsNullOrEmpty(agentRun.Agent.AgentNotification.IsAlivePayload))
                {
                    var payload = agentRun.Agent.AgentNotification.IsAlivePayload.Replace("{{domain}}", agentRun.Subdomain.Name).Replace("{{isAlive}}", scriptOutput.IsAlive.Value.ToString());
                    await this.notificationService.SendAsync(payload, cancellationToken);
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
        private async Task UpdateSubdomainLabel(AgentRun agentRun, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(scriptOutput.Label) &&
                !agentRun.Subdomain.Labels.Any(l => scriptOutput.Label.Equals(l.Label.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var label = await this.labelService.GetByCriteriaAsync(l => l.Name.ToLower() == scriptOutput.Label.ToLower());
                if (label == null)
                {
                    var random = new Random();
                    label = new Label
                    {
                        Name = scriptOutput.Label,
                        Color = string.Format("#{0:X6}", random.Next(0x1000000))
                    };
                }

                agentRun.Subdomain.Labels.Add(new SubdomainLabel
                {
                    Label = label,
                    SubdomainId = agentRun.Subdomain.Id
                });
            }
        }

        /// <summary>
        /// Update the subdomain agent property with the agent name if was updated before
        /// </summary>
        /// <param name="agentRun">The agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        private void UpdateSubdomainAgent(AgentRun agentRun, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(agentRun.Subdomain.FromAgents))
            {
                agentRun.Subdomain.FromAgents = agentRun.Agent.Name;
            }
            else if (!agentRun.Subdomain.FromAgents.Contains(agentRun.Agent.Name))
            {
                agentRun.Subdomain.FromAgents = string.Join(", ", agentRun.Subdomain.FromAgents, agentRun.Agent.Name);
            }
        }
    }
}
