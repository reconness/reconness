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
    public class SubdomainService : Service<Subdomain>, IService<Subdomain>, ISubdomainService, ISaveTerminalOutputParseService
    {
        private readonly ILabelService labelService;
        private readonly INotesService notesService;
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
            INotesService notesService,
            INotificationService notificationService)
            : base(unitOfWork)
        {
            this.labelService = labelService;
            this.notesService = notesService;
            this.notificationService = notificationService;
        }

        /// <summary>
        /// <see cref="ISubdomainService.GetSubdomainsWithIncludesAsync(Target RootDomain, string, CancellationToken)"/>
        /// </summary>
        public async Task<List<Subdomain>> GetSubdomainsWithIncludesAsync(Target target, RootDomain rootDomain, string subdomain, CancellationToken cancellationToken = default)
        {
            target = target ?? throw new ArgumentException("'Target' can not be null");
            rootDomain = rootDomain ?? throw new ArgumentException("'RootDomain' can not be null");

            IQueryable<Subdomain> query;
            if (string.IsNullOrEmpty(subdomain))
            {
                query = this.GetAllQueryableByCriteria(s => s.Target == target && s.RootDomain == rootDomain, cancellationToken);
            }
            else
            {
                query = this.GetAllQueryableByCriteria(s => s.Target == target && s.RootDomain == rootDomain && s.Name == subdomain, cancellationToken);
            }

            return await query
                .Include(t => t.Services)
                .Include(t => t.Notes)
                .Include(t => t.ServiceHttp)
                    .ThenInclude(sh => sh.Directories)
                .Include(t => t.Labels)
                    .ThenInclude(ac => ac.Label)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="ISubdomainService.GetSubdomainsWithIncludesAsync(Target RootDomain, string, CancellationToken)"/>
        /// </summary>
        public async Task<Subdomain> GetSubdomainAsync(Target target, RootDomain rootDomain, string subdomain, CancellationToken cancellationToken = default)
        {
            target = target ?? throw new ArgumentException("'Target' can not be null");
            rootDomain = rootDomain ?? throw new ArgumentException("'RootDomain' can not be null");

            if (string.IsNullOrEmpty(subdomain))
            {
                return null;
            }

            return await this.GetAllQueryableByCriteria(s => s.Target == target && s.RootDomain == rootDomain && s.Name == subdomain, cancellationToken)
                .Include(t => t.Services)
                .Include(t => t.Notes)
                .Include(t => t.ServiceHttp)
                    .ThenInclude(sh => sh.Directories)
                .Include(t => t.Labels)
                    .ThenInclude(ac => ac.Label)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
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
        /// <see cref="ISubdomainService.UpdateSubdomainAgentAsync(Subdomain, string, CancellationToken)"/>
        /// </summary>
        public async Task UpdateSubdomainAgentAsync(Subdomain subdomain, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(subdomain.FromAgents))
            {
                subdomain.FromAgents = agentName;
                await this.UpdateAsync(subdomain, cancellationToken);
            }
            else if (!subdomain.FromAgents.Contains(agentName))
            {
                subdomain.FromAgents = string.Join(", ", subdomain.FromAgents, agentName);
                await this.UpdateAsync(subdomain, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="ISaveTerminalOutputParseService.SaveTerminalOutputParseAsync(agentRunnerner, ScriptOutput, CancellationToken)"/>
        /// </summary>
        public async Task SaveTerminalOutputParseAsync(AgentRunner agentRunner, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(terminalOutputParse.Ip))
            {
                await this.UpdateSubdomainIpAddressAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse, cancellationToken);
            }

            if (terminalOutputParse.IsAlive != null)
            {
                await this.UpdateSubdomainIsAliveAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse, cancellationToken);
            }

            if (terminalOutputParse.HasHttpOpen != null)
            {
                await this.UpdateSubdomainHasHttpOpenAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse, cancellationToken);
            }

            if (terminalOutputParse.Takeover != null)
            {
                await this.UpdateSubdomainTakeoverAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.HttpDirectory))
            {
                await this.UpdateSubdomainDirectoryAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.Service))
            {
                await this.UpdateSubdomainServiceAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.Note))
            {
                await this.UpdateSubdomainNoteAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.HttpScreenshotFilePath) || !string.IsNullOrEmpty(terminalOutputParse.HttpsScreenshotFilePath))
            {
                await this.UpdateSubdomainScreenshotAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse);
            }

            if (!string.IsNullOrWhiteSpace(terminalOutputParse.Label))
            {
                await this.UpdateSubdomainLabelAsync(agentRunner.Subdomain, agentRunner, terminalOutputParse, cancellationToken);
            }

            await this.UpdateSubdomainAgentAsync(agentRunner.Subdomain, agentRunner.Agent.Name, cancellationToken);
        }

        /// <summary>
        /// Assign Ip address to the subdomain
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainIpAddressAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (Helpers.Helpers.ValidateIPv4(scriptOutput.Ip) && subdomain.IpAddress != scriptOutput.Ip)
            {
                subdomain.IpAddress = scriptOutput.Ip;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (agentRunner.ActivateNotification)
                {
                    await this.notificationService.SendAsync(NotificationType.IP, new[]
                    {
                        ("{{domain}}", subdomain.Name),
                        ("{{ip}}", scriptOutput.Ip)
                    }, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain if is Alive
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainIsAliveAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (subdomain.IsAlive != scriptOutput.IsAlive)
            {
                subdomain.IsAlive = scriptOutput.IsAlive.Value;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (agentRunner.ActivateNotification && subdomain.IsAlive.Value)
                {
                    await this.notificationService.SendAsync(NotificationType.IS_ALIVE, new[]
                    {
                        ("{{domain}}", subdomain.Name),
                        ("{{isAlive}}", scriptOutput.IsAlive.Value.ToString())
                    }, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain if it has http port open
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainHasHttpOpenAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (subdomain.HasHttpOpen != scriptOutput.HasHttpOpen.Value)
            {
                subdomain.HasHttpOpen = scriptOutput.HasHttpOpen.Value;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (agentRunner.ActivateNotification && subdomain.HasHttpOpen.Value)
                {
                    await this.notificationService.SendAsync(NotificationType.HAS_HTTP_OPEN, new[]
                    {
                        ("{{domain}}", subdomain.Name),
                        ("{{httpOpen}}", scriptOutput.HasHttpOpen.Value.ToString())
                    }, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain if it can be takeover
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainTakeoverAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (subdomain.Takeover != scriptOutput.Takeover.Value)
            {
                subdomain.Takeover = scriptOutput.Takeover.Value;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (agentRunner.ActivateNotification && subdomain.Takeover.Value)
                {
                    await this.notificationService.SendAsync(NotificationType.TAKEOVER, new[]
                    {
                        ("{{domain}}", subdomain.Name),
                        ("{{takeover}}", scriptOutput.Takeover.Value.ToString())
                    }, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain with directory discovery
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainDirectoryAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
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
            await this.UpdateAsync(subdomain, cancellationToken);

            if (agentRunner.ActivateNotification)
            {
                await this.notificationService.SendAsync(NotificationType.DIRECTORY, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{directory}}", httpDirectory)
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain if is a new service with open port
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainServiceAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (subdomain.Services == null)
            {
                subdomain.Services = new List<Service>();
            }

            var service = new Service
            {
                Name = scriptOutput.Service.ToLower(),
                Port = scriptOutput.Port.Value
            };

            if (!subdomain.Services.Any(s => s.Name == service.Name && s.Port == service.Port))
            {
                subdomain.Services.Add(service);
                await this.UpdateAsync(subdomain, cancellationToken);

                if (agentRunner.ActivateNotification)
                {
                    await this.notificationService.SendAsync(NotificationType.SERVICE, new[]
                    {
                        ("{{domain}}", subdomain.Name),
                        ("{{service}}", service.Name),
                        ("{{port}}", service.Port.ToString())
                    }, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain with screenshots
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        private async Task UpdateSubdomainScreenshotAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(scriptOutput.HttpScreenshotFilePath))
            {
                try
                {
                    var fileBase64 = Convert.ToBase64String(File.ReadAllBytes(scriptOutput.HttpScreenshotFilePath));
                    if (subdomain.ServiceHttp == null)
                    {
                        subdomain.ServiceHttp = new ServiceHttp();
                    }

                    subdomain.ServiceHttp.ScreenshotHttpPNGBase64 = fileBase64;
                    await this.UpdateAsync(subdomain, cancellationToken);

                    if (agentRunner.ActivateNotification)
                    {
                        await this.notificationService.SendAsync(NotificationType.SCREENSHOT, new[]
                        {
                            ("{{domain}}", subdomain.Name),
                            ("{{httpScreenshot}}", "true"),
                            ("{{httpsScreenshot}}", "false")
                        }, cancellationToken);
                    }
                }
                catch
                {

                }
            }

            if (!string.IsNullOrEmpty(scriptOutput.HttpsScreenshotFilePath))
            {
                try
                {
                    var fileBase64 = Convert.ToBase64String(File.ReadAllBytes(scriptOutput.HttpsScreenshotFilePath));
                    if (subdomain.ServiceHttp == null)
                    {
                        subdomain.ServiceHttp = new ServiceHttp();
                    }

                    subdomain.ServiceHttp.ScreenshotHttpsPNGBase64 = fileBase64;
                    await this.UpdateAsync(subdomain, cancellationToken);

                    if (agentRunner.ActivateNotification)
                    {
                        await this.notificationService.SendAsync(NotificationType.SCREENSHOT, new[]
                        {
                            ("{{domain}}", subdomain.Name),
                            ("{{httpScreenshot}}", "false"),
                            ("{{httpsScreenshot}}", "true")
                        }, cancellationToken);
                    }
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Update the subdomain Note
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainNoteAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken)
        {
            var notes = scriptOutput.Note;
            if (!string.IsNullOrEmpty(subdomain.Notes?.Notes ?? string.Empty))
            {
                notes = subdomain.Notes.Notes + "\n" + notes;
            }

            await this.notesService.SaveSubdomainNotesAsync(subdomain, notes, cancellationToken);

            if (agentRunner.ActivateNotification)
            {
                await this.notificationService.SendAsync(NotificationType.NOTE, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{note}}", scriptOutput.Note)
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain label
        /// </summary>
        /// <param name="agentRunner">The Agent</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainLabelAsync(Subdomain subdomain, AgentRunner agentRunner, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (!subdomain.Labels.Any(l => scriptOutput.Label.Equals(l.Label.Name, StringComparison.OrdinalIgnoreCase)))
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

                await this.UpdateAsync(subdomain, cancellationToken);
            }
        }
    }
}
