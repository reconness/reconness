using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ISubdomainService"/> 
    /// </summary>
    public class SubdomainService : Service<Subdomain>, IService<Subdomain>, ISubdomainService
    {
        private readonly ILabelService labelService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISubdomainService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public SubdomainService(
            IUnitOfWork unitOfWork,
            ILabelService labelService)
            : base(unitOfWork)
        {
            this.labelService = labelService;
        }

        /// <summary>
        /// <see cref="ISubdomainService.UpdateSubdomainAsync(Subdomain, Agent, ScriptOutput, bool)"/>
        /// </summary>
        public async Task UpdateSubdomain(Subdomain subdomain, Agent agent, ScriptOutput scriptOutput)
        {
            this.UpdateSubdomainIpAddress(subdomain, scriptOutput)
                .UpdateSubdomainIsAlive(subdomain, scriptOutput)
                .UpdateSubdomainHasHttpOpen(subdomain, scriptOutput)
                .UpdateSubdomainTakeover(subdomain, scriptOutput)
                .UpdateSubdomainScreenshot(subdomain, scriptOutput)
                .UpdateSubdomainDirectory(subdomain, scriptOutput)
                .UpdateSubdomainService(subdomain, scriptOutput)
                .UpdateSubdomainAgent(subdomain, agent);

            await UpdateSubdomainLabel(subdomain, scriptOutput);

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
        /// Assign Ip address to the subdomain
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns><see cref="ISubdomainService"/></returns>
        private SubdomainService UpdateSubdomainIpAddress(Subdomain subdomain, ScriptOutput scriptOutput)
        {
            if (scriptOutput.Ip != null && Helpers.Helpers.ValidateIPv4(scriptOutput.Ip) && subdomain.IpAddress != scriptOutput.Ip)
            {
                subdomain.IpAddress = scriptOutput.Ip;
            }

            return this;
        }

        /// <summary>
        /// Update the subdomain if it has http port open
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns><see cref="ISubdomainService"/></returns>
        private SubdomainService UpdateSubdomainHasHttpOpen(Subdomain subdomain, ScriptOutput scriptOutput)
        {
            if (scriptOutput.HasHttpOpen != null && subdomain.HasHttpOpen != scriptOutput.HasHttpOpen.Value)
            {
                subdomain.HasHttpOpen = scriptOutput.HasHttpOpen.Value;
            }

            return this;
        }

        /// <summary>
        /// Update the subdomain if it can be takeover
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns><see cref="ISubdomainService"/></returns>
        private SubdomainService UpdateSubdomainTakeover(Subdomain subdomain, ScriptOutput scriptOutput)
        {
            if (scriptOutput.Takeover != null && subdomain.Takeover != scriptOutput.Takeover.Value)
            {
                subdomain.Takeover = scriptOutput.Takeover.Value;
            }

            return this;
        }

        /// <summary>
        /// Update the subdomain with screenshots
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns><see cref="ISubdomainService"/></returns>

        private SubdomainService UpdateSubdomainScreenshot(Subdomain subdomain, ScriptOutput scriptOutput)
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
                catch (Exception ex)
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
                catch (Exception ex)
                {

                }
            }

            return this;
        }

        /// <summary>
        /// Update the subdomain with directory discovery
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns><see cref="ISubdomainService"/></returns>
        private SubdomainService UpdateSubdomainDirectory(Subdomain subdomain, ScriptOutput scriptOutput)
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
                    return this;
                }

                var directory = new ServiceHttpDirectory()
                {
                    Directory = httpDirectory,
                    StatusCode = scriptOutput.HttpDirectoryStatusCode,
                    Method = scriptOutput.HttpDirectoryMethod,
                    Size = scriptOutput.HttpDirectorySize
                };

                subdomain.ServiceHttp.Directories.Add(directory);
            }

            return this;
        }

        /// <summary>
        /// Update the subdomain if is a new service with open port
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns><see cref="ISubdomainService"/></returns>
        private SubdomainService UpdateSubdomainService(Subdomain subdomain, ScriptOutput scriptOutput)
        {
            if (string.IsNullOrWhiteSpace(scriptOutput.Service))
            {
                return this;
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
            }

            return this;
        }

        /// <summary>
        /// Update the subdomain label
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns><see cref="ISubdomainService"/></returns>
        private async Task UpdateSubdomainLabel(Subdomain subdomain, ScriptOutput scriptOutput)
        {
            if (!string.IsNullOrWhiteSpace(scriptOutput.Label) &&
                !subdomain.Labels.Any(l => scriptOutput.Label.Equals(l.Label.Name, StringComparison.OrdinalIgnoreCase)))
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

                subdomain.Labels.Add(new SubdomainLabel
                {
                    Label = label,
                    SubdomainId = subdomain.Id
                });
            }
        }

        /// <summary>
        /// Update the subdomain if is Alive
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="scriptOutput">The terminal output one line</param>
        /// <returns><see cref="ISubdomainService"/></returns>
        private SubdomainService UpdateSubdomainIsAlive(Subdomain subdomain, ScriptOutput scriptOutput)
        {
            if (scriptOutput.IsAlive != null && subdomain.IsAlive != scriptOutput.IsAlive)
            {
                subdomain.IsAlive = scriptOutput.IsAlive.Value;
            }

            return this;
        }

        /// <summary>
        /// Update the subdomain agent property with the agent name if was updated before
        /// </summary>
        /// <param name="subdomain">The subdomain</param>
        /// <param name="agent">The agent</param>
        private SubdomainService UpdateSubdomainAgent(Subdomain subdomain, Agent agent)
        {
            if (string.IsNullOrWhiteSpace(subdomain.FromAgents))
            {
                subdomain.FromAgents = agent.Name;
            }
            else if (!subdomain.FromAgents.Contains(agent.Name))
            {
                subdomain.FromAgents = string.Join(", ", subdomain.FromAgents, agent.Name);
            }

            return this;
        }
    }
}
