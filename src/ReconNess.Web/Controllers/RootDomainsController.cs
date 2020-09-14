using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RootDomainsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITargetService targetService;
        private readonly IRootDomainService rootDomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        /// <param name="targetService"><see cref="ITargetService"/></param>
        public RootDomainsController(
            IMapper mapper,
            ITargetService targetService,
            IRootDomainService rootDomainService)
        {
            this.mapper = mapper;
            this.targetService = targetService;
            this.rootDomainService = rootDomainService;
        }

        // GET api/rootdomains/{targetName}/{rootDomainName}
        [HttpGet("{targetName}/{rootDomainName}")]
        public async Task<IActionResult> Get(string targetName, string rootDomainName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            target.RootDomains = new List<RootDomain> { await rootDomainService.GetDomainWithSubdomainsAsync(r => r.Name == rootDomainName && r.Target == target, cancellationToken) };

            return Ok(this.mapper.Map<Target, TargetDto>(target));
        }

        // DELETE api/rootdomains/{targetName}/{rootDomainName}
        [HttpDelete("{targetName}/{rootDomainName}")]
        public async Task<IActionResult> DeleteSubdomains(string targetName, string rootDomainName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetDomainWithSubdomainsAsync(t => t.Name == rootDomainName && t.Target == target, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            await this.rootDomainService.DeleteSubdomainsAsync(rootDomain, cancellationToken);

            return NoContent();
        }

        // POST api/rootdomains/{targetName}/{rootDomainName}
        [HttpPost("{targetName}/{rootDomainName}")]
        public async Task<IActionResult> Upload(string targetName, string rootDomainName, IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length == 0)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetDomainWithSubdomainsAsync(t => t.Name == rootDomainName && t.Target == target, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            try
            {
                var path = Path.GetTempFileName();
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var json = System.IO.File.ReadAllLines(path).FirstOrDefault();
                var rootDomainDto = JsonConvert.DeserializeObject<RootDomainDto>(json);

                var uploadRootDomain = this.mapper.Map<RootDomainDto, RootDomain>(rootDomainDto);

                var subdomainsAdded = await this.rootDomainService.UploadRootDomainAsync(rootDomain, uploadRootDomain, cancellationToken);

                return Ok(this.mapper.Map<List<Subdomain>, List<SubdomainDto>>(subdomainsAdded));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/rootdomains/uploadSubdomains/{targetName}/{rootDomainName}
        [HttpPost("uploadSubdomains/{targetName}/{rootDomainName}")]
        public async Task<IActionResult> UploadSubdomains(string targetName, string rootDomainName, IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length == 0)
            {
                return BadRequest();
            }

            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetDomainWithSubdomainsAsync(t => t.Name == rootDomainName && t.Target == target, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            try
            {
                var path = Path.GetTempFileName();
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var subdomains = System.IO.File.ReadAllLines(path).ToList();
                var subdomainsAdded = await this.rootDomainService.UploadSubdomainsAsync(rootDomain, subdomains);

                return Ok(this.mapper.Map<List<Subdomain>, List<SubdomainDto>>(subdomainsAdded));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/rootdomains/export/{targetName}/{rootDomainName}
        [HttpPost("export/{targetName}/{rootDomainName}")]
        public async Task<IActionResult> Donwload(string targetName, string rootDomainName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetDomainWithSubdomainsAsync(t => t.Name == rootDomainName && t.Target == target, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var domainDto = this.mapper.Map<RootDomain, RootDomainDto>(rootDomain);
            var result = JsonConvert.SerializeObject(domainDto, new JsonSerializerSettings());
            var download = Encoding.UTF8.GetBytes(result); ;

            return File(download, "application/json", "rootdomain.json");
        }

        // GET api/rootdomains/exportSubdomains/{targetName}/{rootDomainName}
        [HttpPost("exportSubdomains/{targetName}/{rootDomainName}")]
        public async Task<IActionResult> DonwloadSubdomains(string targetName, string rootDomainName, CancellationToken cancellationToken)
        {
            var target = await this.targetService.GetByCriteriaAsync(t => t.Name == targetName, cancellationToken);
            if (target == null)
            {
                return NotFound();
            }

            var rootDomain = await this.rootDomainService.GetDomainWithSubdomainsAsync(t => t.Name == rootDomainName && t.Target == target, cancellationToken);
            if (rootDomain == null)
            {
                return NotFound();
            }

            var data = string.Join(",", rootDomain.Subdomains.Select(s => s.Name));

            var download = Encoding.UTF8.GetBytes(data);

            return File(download, "text/csv", "subdomains.csv");
        }
    }
}
