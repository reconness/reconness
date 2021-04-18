using Microsoft.EntityFrameworkCore;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Data.Npgsql.Helpers;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ISubdomainService"/> 
    /// </summary>
    public class SubdomainService : Service<Subdomain>, IService<Subdomain>, ISubdomainService, ISaveTerminalOutputParseService<Subdomain>
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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

        /// <inheritdoc/>
        public async Task<List<Subdomain>> GetSubdomainsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(predicate)
                    .Include(s => s.RootDomain)
                    .Include(s => s.Services)
                    .Include(s => s.Notes)
                    .Include(s => s.Directories)
                    .Include(s => s.Labels)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subdomain> GetSubdomainAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(predicate)
                    .Include(t => t.Services)
                    .Include(t => t.Notes)
                    .Include(t => t.Directories)
                    .Include(t => t.Labels)
                .SingleAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subdomain> GetSubdomainNoTrackingAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(predicate)
                    .Include(t => t.Services)
                    .Include(t => t.Notes)
                    .Include(t => t.Directories)
                    .Include(t => t.Labels)
                    .AsNoTracking()
                .SingleAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<PagedResult<Subdomain>> GetPaginateAsync(RootDomain rootDomain, string query, int page, int limit, CancellationToken cancellationToken = default)
        {
            IQueryable<Subdomain> queryable = default;
            if (string.IsNullOrEmpty(query))
            {
                queryable = this.GetAllQueryableByCriteria(s => s.RootDomain == rootDomain)
                        .Select(subdomain => new Subdomain
                        {
                            Id = subdomain.Id,
                            Name = subdomain.Name,
                            CreatedAt = subdomain.CreatedAt,
                            IpAddress = subdomain.IpAddress,
                            AgentsRanBefore = subdomain.AgentsRanBefore,
                            HasHttpOpen = subdomain.HasHttpOpen,
                            IsAlive = subdomain.IsAlive,
                            IsMainPortal = subdomain.IsMainPortal,
                            Takeover = subdomain.Takeover,
                            Labels = subdomain.Labels
                                    .Select(label => new Label
                                    {
                                        Name = label.Name,
                                        Color = label.Color
                                    })
                                    .ToList(),
                            Services = subdomain.Services
                                    .Select(service => new Service
                                    {
                                        Name = service.Name
                                    }).ToList()
                        })
                    .OrderByDescending(s => s.CreatedAt)
                    .AsNoTracking();
            }
            else
            {
                queryable = this.GetAllQueryableByCriteria(s => s.RootDomain == rootDomain && s.Name.Contains(query))
                    .Select(subdomain => new Subdomain
                    {
                        Name = subdomain.Name,
                        CreatedAt = subdomain.CreatedAt,
                        IpAddress = subdomain.IpAddress,
                        AgentsRanBefore = subdomain.AgentsRanBefore,
                        HasHttpOpen = subdomain.HasHttpOpen,
                        IsAlive = subdomain.IsAlive,
                        IsMainPortal = subdomain.IsMainPortal,
                        Takeover = subdomain.Takeover,
                        Labels = subdomain.Labels
                                    .Select(label => new Label
                                    {
                                        Name = label.Name,
                                        Color = label.Color
                                    })
                                    .ToList(),
                        Services = subdomain.Services
                                    .Select(service => new Service
                                    {
                                        Name = service.Name
                                    }).ToList()
                    })
                    .OrderByDescending(s => s.CreatedAt)
                    .AsNoTracking();
            }

            return await queryable.GetPageAsync<Subdomain>(page, limit, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subdomain> GetWithLabelsAsync(Expression<Func<Subdomain, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.GetAllQueryableByCriteria(predicate)
                    .Include(t => t.Labels)
                .SingleAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddLabelAsync(Subdomain subdomain, string newLabel, CancellationToken cancellationToken = default)
        {
            var myLabels = subdomain.Labels.Select(l => l.Name).ToList();
            myLabels.Add(newLabel);

            subdomain.Labels = await this.labelService.GetLabelsAsync(subdomain.Labels, myLabels, cancellationToken);

            await this.UpdateAsync(subdomain, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAgentRanAsync(Subdomain subdomain, string agentName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(subdomain.AgentsRanBefore))
            {
                subdomain.AgentsRanBefore = agentName;
                await this.UpdateAsync(subdomain, cancellationToken);
            }
            else if (!subdomain.AgentsRanBefore.Contains(agentName))
            {
                subdomain.AgentsRanBefore = string.Join(", ", subdomain.AgentsRanBefore, agentName);
                await this.UpdateAsync(subdomain, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task SaveTerminalOutputParseAsync(Subdomain subdomain, string agentName, bool activateNotification, ScriptOutput terminalOutputParse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // if we have a new subdomain
            if (!string.IsNullOrEmpty(terminalOutputParse.Subdomain) && !subdomain.Name.Equals(terminalOutputParse.Subdomain))
            {
                subdomain = new Subdomain
                {
                    Name = terminalOutputParse.Subdomain,
                    AgentsRanBefore = agentName,
                    RootDomain = subdomain.RootDomain
                };

                await this.AddAsync(subdomain, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.Ip))
            {
                await this.UpdateSubdomainIpAddressAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (terminalOutputParse.IsAlive != null)
            {
                await this.UpdateSubdomainIsAliveAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (terminalOutputParse.HasHttpOpen != null)
            {
                await this.UpdateSubdomainHasHttpOpenAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (terminalOutputParse.Takeover != null)
            {
                await this.UpdateSubdomainTakeoverAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.HttpDirectory))
            {
                await this.UpdateSubdomainDirectoryAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.Service))
            {
                await this.UpdateSubdomainServiceAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.Note))
            {
                await this.UpdateSubdomainNoteAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.Technology))
            {
                await this.UpdateSubdomainTechnologyAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrEmpty(terminalOutputParse.HttpScreenshotFilePath) || !string.IsNullOrEmpty(terminalOutputParse.HttpsScreenshotFilePath))
            {
                await this.UpdateSubdomainScreenshotAsync(subdomain, activateNotification, terminalOutputParse, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(terminalOutputParse.Label))
            {
                await this.UpdateSubdomainLabelAsync(subdomain, terminalOutputParse, cancellationToken);
            }
        }

        /// <summary>
        /// Assign Ip address to the subdomain
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainIpAddressAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (Helpers.Helpers.ValidateIPv4(scriptOutput.Ip) && subdomain.IpAddress != scriptOutput.Ip)
            {
                subdomain.IpAddress = scriptOutput.Ip;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (activateNotification)
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
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainIsAliveAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (subdomain.IsAlive != scriptOutput.IsAlive)
            {
                subdomain.IsAlive = scriptOutput.IsAlive.Value;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (activateNotification && subdomain.IsAlive.Value)
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
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainHasHttpOpenAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (subdomain.HasHttpOpen != scriptOutput.HasHttpOpen.Value)
            {
                subdomain.HasHttpOpen = scriptOutput.HasHttpOpen.Value;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (activateNotification && subdomain.HasHttpOpen.Value)
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
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainTakeoverAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (subdomain.Takeover != scriptOutput.Takeover.Value)
            {
                subdomain.Takeover = scriptOutput.Takeover.Value;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (activateNotification && subdomain.Takeover.Value)
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
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainDirectoryAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            var httpDirectory = scriptOutput.HttpDirectory.TrimEnd('/').TrimEnd();
            if (subdomain.Directories == null)
            {
                subdomain.Directories = new List<Entities.Directory>();
            }


            if (subdomain.Directories.Any(d => d.Uri == httpDirectory))
            {
                return;
            }

            var directory = new Entities.Directory()
            {
                Uri = httpDirectory,
                StatusCode = scriptOutput.HttpDirectoryStatusCode,
                Method = scriptOutput.HttpDirectoryMethod,
                Size = scriptOutput.HttpDirectorySize
            };

            subdomain.Directories.Add(directory);
            await this.UpdateAsync(subdomain, cancellationToken);

            if (activateNotification)
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
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainServiceAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
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

                if (activateNotification)
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
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        private async Task UpdateSubdomainScreenshotAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(scriptOutput.HttpScreenshotFilePath))
            {
                try
                {
                    var fileBase64 = Convert.ToBase64String(File.ReadAllBytes(scriptOutput.HttpScreenshotFilePath));

                    subdomain.ScreenshotHttpPNGBase64 = fileBase64;
                    await this.UpdateAsync(subdomain, cancellationToken);

                    if (activateNotification)
                    {
                        await this.notificationService.SendAsync(NotificationType.SCREENSHOT, new[]
                        {
                            ("{{domain}}", subdomain.Name),
                            ("{{httpScreenshot}}", "true"),
                            ("{{httpsScreenshot}}", "false")
                        }, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(scriptOutput.HttpsScreenshotFilePath))
            {
                try
                {
                    var fileBase64 = Convert.ToBase64String(File.ReadAllBytes(scriptOutput.HttpsScreenshotFilePath));

                    subdomain.ScreenshotHttpsPNGBase64 = fileBase64;
                    await this.UpdateAsync(subdomain, cancellationToken);

                    if (activateNotification)
                    {
                        await this.notificationService.SendAsync(NotificationType.SCREENSHOT, new[]
                        {
                            ("{{domain}}", subdomain.Name),
                            ("{{httpScreenshot}}", "false"),
                            ("{{httpsScreenshot}}", "true")
                        }, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// Update the subdomain Note
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainNoteAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken)
        {
            var notes = scriptOutput.Note;
            if (!string.IsNullOrEmpty(subdomain.Notes?.Notes ?? string.Empty))
            {
                notes = subdomain.Notes.Notes + "\n" + notes;
            }

            await this.notesService.SaveSubdomainNotesAsync(subdomain, notes, cancellationToken);

            if (activateNotification)
            {
                await this.notificationService.SendAsync(NotificationType.NOTE, new[]
                {
                    ("{{domain}}", subdomain.Name),
                    ("{{note}}", scriptOutput.Note)
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Update the subdomain Technology
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="activateNotification">If we need to send notificationt</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainTechnologyAsync(Subdomain subdomain, bool activateNotification, ScriptOutput scriptOutput, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(scriptOutput.Technology) && !scriptOutput.Technology.Equals(subdomain.Technology, StringComparison.OrdinalIgnoreCase))
            {
                subdomain.Technology = scriptOutput.Technology;
                await this.UpdateAsync(subdomain, cancellationToken);

                if (activateNotification && subdomain.Takeover.Value)
                {
                    await this.notificationService.SendAsync(NotificationType.TECHNOLOGY, new[]
                    {
                        ("{{domain}}", subdomain.Name),
                        ("{{technology}}", scriptOutput.Technology)
                    }, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Update the subdomain label
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A task</returns>
        private async Task UpdateSubdomainLabelAsync(Subdomain subdomain, ScriptOutput scriptOutput, CancellationToken cancellationToken = default)
        {
            if (!subdomain.Labels.Any(l => scriptOutput.Label.Equals(l.Name, StringComparison.OrdinalIgnoreCase)))
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

                subdomain.Labels.Add(label);

                await this.UpdateAsync(subdomain, cancellationToken);
            }
        }
    }
}
