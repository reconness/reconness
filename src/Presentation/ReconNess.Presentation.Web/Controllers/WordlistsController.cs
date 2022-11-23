using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReconNess.Application.Models;
using ReconNess.Application.Services;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Presentation.Api.Controllers;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class WordlistsController : ControllerBase
{
    private readonly IEventTrackService eventTrackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WordlistsController" /> class
    /// </summary>
    /// <param name="eventTrackService"><see cref="IEventTrackService"/></param>
    public WordlistsController(IEventTrackService eventTrackService)
    {
        this.eventTrackService = eventTrackService;
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
    /// <returns>The notifications configuration</returns>
    /// <response code="200">Returns the list of wordlist, content discovery and resolvers</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Get()
    {
        var subdomainEnumPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", "subdomain_enum");
        var subdomainEnumDirectory = new DirectoryInfo(subdomainEnumPath);//Assuming Test is your Folder
        FileInfo[] subdomainEnumFiles = subdomainEnumDirectory.GetFiles("*.*"); //Getting Text files

        var dirEnumPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", "dir_enum");
        var dirEnumDirectory = new DirectoryInfo(dirEnumPath);//Assuming Test is your Folder
        FileInfo[] dirEnumFiles = dirEnumDirectory.GetFiles("*.*"); //Getting Text files

        var dnsResolverEnumPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", "dns_resolver_enum");
        var dnsResolverEnumDirectory = new DirectoryInfo(dnsResolverEnumPath);//Assuming Test is your Folder
        FileInfo[] dnsResolverEnumFiles = dnsResolverEnumDirectory.GetFiles("*.*"); //Getting Text files

        var result = new WordlistsDto();

        foreach (var subdomainEnumFile in subdomainEnumFiles)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", "subdomain_enum", subdomainEnumFile.Name);
            result.SubdomainsEnum.Add(new Wordlist
            {
                Filename = subdomainEnumFile.Name,
                Path = path,
                Size = subdomainEnumFile.Length.ToString(),
                Count = System.IO.File.ReadLines(path).Count()
            });
        }

        foreach (var dirEnumFile in dirEnumFiles)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", "dir_enum", dirEnumFile.Name);
            result.DirectoriesEnum.Add(new Wordlist
            {
                Filename = dirEnumFile.Name,
                Path = path,
                Size = dirEnumFile.Length.ToString(),
                Count = System.IO.File.ReadLines(path).Count()
            });
        }

        foreach (var dnsResolverEnumFile in dnsResolverEnumFiles)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", "dns_resolver_enum", dnsResolverEnumFile.Name);
            result.DNSResolvers.Add(new Wordlist
            {
                Filename = dnsResolverEnumFile.Name,
                Path = path,
                Size = dnsResolverEnumFile.Length.ToString(),
                Count = System.IO.File.ReadLines(path).Count()
            });
        }

        return Ok(result);
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
    /// <returns>The notifications configuration</returns>
    /// <response code="200">Returns the list of wordlist, content discovery and resolvers</response>
    /// <response code="401">If the user is not authenticate</response>
    [HttpGet("content")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetContent(string type, string filename)
    {
        if (!"dir_enum".Equals(type) && !"dns_resolver_enum".Equals(type) && !"subdomain_enum".Equals(type))
        {
            return BadRequest();
        }

        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            filename = filename.Replace(c.ToString(), "");
        }

        var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", type);
        var path = Path.Combine(configPath, filename);
        if (!path.StartsWith(configPath))
        {
            return BadRequest();
        }

        using var fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var bs = new BufferedStream(fs);
        using var sr = new StreamReader(bs);

        return Ok(new { Data = await sr.ReadToEndAsync(), Path = path });
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
    [HttpGet("download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Download(string type, string filename, CancellationToken cancellationToken)
    {
        if (!"dir_enum".Equals(type) && !"dns_resolver_enum".Equals(type) && !"subdomain_enum".Equals(type))
        {
            return BadRequest();
        }

        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            filename = filename.Replace(c.ToString(), "");
        }

        var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", type);
        var path = Path.Combine(configPath, filename);
        if (!path.StartsWith(configPath))
        {
            return BadRequest();
        }

        await this.eventTrackService.AddAsync(new EventTrack
        {
            Description = $"Wordlist {type}/{filename} downloaded"
        }, cancellationToken);

        return PhysicalFile(path, "application/text");
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
        if (!"dir_enum".Equals(type) && !"dns_resolver_enum".Equals(type) && !"subdomain_enum".Equals(type))
        {
            return BadRequest();
        }

        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            filename = filename.Replace(c.ToString(), "");
        }

        var wordlistPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", type);
        var path = Path.Combine(wordlistPath, filename);

        if (!System.IO.File.Exists(path))
        {
            return BadRequest();
        }

        if (path.StartsWith(wordlistPath))
        {
            await System.IO.File.WriteAllTextAsync(path, wordlistInputDto.Data, cancellationToken);
        }

        await this.eventTrackService.AddAsync(new EventTrack
        {
            Description = $"Wordlist {type}/{filename} edited"
        }, cancellationToken);

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
        if (!"dir_enum".Equals(type) && !"dns_resolver_enum".Equals(type) && !"subdomain_enum".Equals(type))
        {
            return BadRequest();
        }

        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            filename = filename.Replace(c.ToString(), "");
        }

        var wordlistPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", type);
        var path = Path.Combine(wordlistPath, filename);

        if (!System.IO.File.Exists(path))
        {
            return BadRequest();
        }

        if (path.StartsWith(wordlistPath))
        {
            System.IO.File.Delete(path);
        }

        await this.eventTrackService.AddAsync(new EventTrack
        {
            Description = $"Wordlist {type}/{filename} deleted"
        }, cancellationToken);

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
    /// <response code="200">Returns some of the file's metadata</response>
    /// <response code="400">Bad Request</response>
    /// <response code="401">If the user is not authenticate</response>
    /// <response code="404">Not Found</response>
    [HttpPost("upload/{type}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Upload(string type, IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest();
        }

        if (!"dir_enum".Equals(type) && !"dns_resolver_enum".Equals(type) && !"subdomain_enum".Equals(type))
        {
            return BadRequest();
        }

        var filename = file.FileName;
        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalid)
        {
            filename = filename.Replace(c.ToString(), "");
        }

        var wordlistPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "wordlists", type);
        var fileNamePath = Path.Combine(wordlistPath, filename);
        if (!fileNamePath.StartsWith(wordlistPath))
        {
            return BadRequest("The path to save the file is invalid.");
        }

        if (System.IO.File.Exists(fileNamePath))
        {
            return BadRequest($"We have a file with that name inside the folder {type}, please change the filename and try again.");
        }

        using (var stream = new FileStream(fileNamePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        WordlisFileMetadataDto wordlistMetadata = new WordlisFileMetadataDto();
        wordlistMetadata.Filename = file.FileName;
        wordlistMetadata.Count = System.IO.File.ReadLines(fileNamePath).Count();
        wordlistMetadata.Size = file.Length.ToString();
        wordlistMetadata.Path = fileNamePath;

        await this.eventTrackService.AddAsync(new EventTrack
        {
            Description = $"Wordlist {type}/{filename} uploaded"
        }, cancellationToken);

        return Ok(wordlistMetadata);
    }
}
