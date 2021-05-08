using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Core.Models;
using ReconNess.Web.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class WordlistsController : ControllerBase
    {
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordlistsController" /> class
        /// </summary>
        /// <param name="mapper"><see cref="IMapper"/></param>
        public WordlistsController(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Obtain the list of wordlist, content discovery and resolvers.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/wordlists
        ///
        /// </remarks>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="200">Returns the list of wordlist, content discovery and resolvers</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(new WordlistsDto
            {
                SubdomainsEnum = new List<Wordlist>
                {
                    new Wordlist
                    {
                        Filename = "Default.txt",
                        Count = 18484,
                        Size = "2.5mb",
                        Path = "/app/Content/wordlists/subdomain_enum/Default.txt"
                    },
                    new Wordlist
                    {
                        Filename = "Default1.txt",
                        Count = 34484,
                        Size = "5.1mb",
                        Path = "/app/Content/wordlists/subdomain_enum/Default1.txt"
                    },
                    new Wordlist
                    {
                        Filename = "Default2.txt",
                        Count = 2484,
                        Size = "504kb",
                        Path = "/app/Content/wordlists/subdomain_enum/Default2.txt"
                    }
                },
                DirectoriesEnum = new List<Wordlist>
                {
                    new Wordlist
                    {
                        Filename = "Default.txt",
                        Count = 18484,
                        Size = "2.5mb",
                        Path = "/app/Content/wordlists/dir_enum/Default.txt"
                    },
                    new Wordlist
                    {
                        Filename = "Default1.txt",
                        Count = 34484,
                        Size = "5.1mb",
                        Path = "/app/Content/wordlists/dir_enum/Default1.txt"
                    }
                },
                DNSResolvers = new List<Wordlist>
                {
                    new Wordlist
                    {
                        Filename = "Default.txt",
                        Count = 18484,
                        Size = "2.5mb",
                        Path = "/app/Content/wordlists/dns_resolver_enum/Default.txt"
                    },
                    new Wordlist
                    {
                        Filename = "Default1.txt",
                        Count = 34484,
                        Size = "5.1mb",
                        Path = "/app/Content/wordlists/dns_resolver_enum/Default1.txt"
                    }
                }
            });
        }

        /// <summary>
        /// Obtain the content of the wordlist based in the type, can be [subdomain_enum, dir_enum, dns_resolver_enum].
        /// That allow us know what folder we need to look for the file. /app/Content/wordlist/[type]/[filename]
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/wordlists/data/{type}/{filename}
        ///
        /// </remarks>
        /// <param name="type">The type [subdomain_enum, dir_enum, dns_resolver_enum]</param>
        /// <param name="filename">The filename</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The notifications configuration</returns>
        /// <response code="200">Returns the list of wordlist, content discovery and resolvers</response>
        /// <response code="401">If the user is not authenticate</response>
        [HttpGet("content")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetContent(string type, string filename, CancellationToken cancellationToken)
        {
            await Task.Delay(500);

            return Ok("Hello Word!");
        }

        /// <summary>
        /// Update the wordlist content based in the type [subdomain_enum, dir_enum, dns_resolver_enum].
        /// That allow us know what folder we need to look for the file. /app/Content/wordlist/[type]/[filename]
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/wordlists/{type}/{filename}
        ///     { 
        ///         "data": "...data..."
        ///     }
        ///
        /// </remarks>
        /// <param name="type">The type [subdomain_enum, dir_enum, dns_resolver_enum]</param>
        /// <param name="filename">The filename</param>
        /// <param name="wordlistInputDto">The data to save</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPut("{type}/{filename}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(string type, string filename, [FromBody] WordlistInputDto wordlistInputDto, CancellationToken cancellationToken)
        {
            await Task.Delay(500);

            return NoContent();
        }

        /// <summary>
        /// Delete a file based in the type [subdomain_enum, dir_enum, dns_resolver_enum].
        /// That allow us know what folder we need to look for the file. /app/Content/wordlist/[type]/[filename]
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/wordlists/{type}/{filename}
        ///
        /// </remarks>
        /// <param name="type">The type [subdomain_enum, dir_enum, dns_resolver_enum]</param>
        /// <param name="filename">The filename</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate or is not admin</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{type}/{filename}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string type, string filename, CancellationToken cancellationToken)
        {
            await Task.Delay(500);

            return NoContent();
        }

        /// <summary>
        /// Upload the file with the wordlist based in the type [subdomain_enum, dir_enum, dns_resolver_enum].
        /// That allow us know what folder we need to look for the file. /app/Content/wordlist/[type]/[filename]
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/wordlists/upload/{type}
        ///
        /// </remarks>
        /// <param name="type">The type [subdomain_enum, dir_enum, dns_resolver_enum]</param>
        /// <param name="file">The file with all the wordlist</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <response code="204">No Content</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">If the user is not authenticate</response>
        /// <response code="404">Not Found</response>
        [HttpPost("upload/{type}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Upload(string type, IFormFile file, CancellationToken cancellationToken)
        {
            await Task.Delay(500);

            return NoContent();
        }
    }
}
