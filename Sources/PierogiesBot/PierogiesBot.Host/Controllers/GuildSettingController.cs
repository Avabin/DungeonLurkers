using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PierogiesBot.Persistence.GuildSettings.Features;
using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Persistence.Core.Features.Exceptions;

namespace PierogiesBot.Host.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class GuildSettingController : ControllerBase
{
    private readonly IGuildSettingFacade _guildSettingFacade;
    
    public GuildSettingController(IGuildSettingFacade guildSettingFacade)
#pragma warning restore CS1591
    {
        _guildSettingFacade = guildSettingFacade;
    }
    
    // GET: get all with nullable skip and limit
    /// <summary>
    ///     Fetches all guilds settings
    /// </summary>
    /// <param name="skip">Count of objects to skip from start</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of all guilds settings</returns>
    /// <response code="200">Returns all guilds settings</response>
    [ProducesResponseType(typeof(IEnumerable<GuildSettingDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetAll(int? skip, int? limit)
    {
        var guildSettings = await _guildSettingFacade.GetAllAsync(skip, limit);
        return Ok(guildSettings);
    }
    
    // GET: find guild setting by (ulong) guild id with nullable skip and limit
    /// <summary>
    ///     Fetches guild setting by guild id
    /// </summary>
    /// <param name="guildId">Guild id</param>
    /// <returns>Guild setting</returns>
    /// <response code="200">Returns guild setting</response>
    /// <response code="404">Guild setting not found</response>
    [ProducesResponseType(typeof(GuildSettingDto), StatusCodes.Status200OK)]
    [HttpGet("guild/{guildId}")]
    public async Task<IActionResult> Get(ulong guildId)
    {
        var guildSetting = await _guildSettingFacade.FindByGuildId(guildId);
        if (guildSetting == null)
        {
            return NotFound();
        }

        return Ok(guildSetting);
    }

    // GET: find guild setting by id
    /// <summary>
    ///     Fetches guild setting by id
    /// </summary>
    /// <param name="id">Id of guild setting</param>
    /// <returns>Guild setting</returns>
    /// <response code="200">Returns guild setting</response>
    /// <response code="400">If guild setting is not valid or if ID is not a valid ObjectId</response>
    /// <response code="404">If guild setting is not found</response>
    [ProducesResponseType(typeof(GuildSettingDto), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<IActionResult> FindById(string id)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");
        
        var guildSetting = await _guildSettingFacade.GetByIdAsync(id);
        if (guildSetting is null)
        {
            return NotFound();
        }
        return Ok(guildSetting);
    }
    
    // POST: Create guild setting
    /// <summary>
    ///     Creates new guild setting
    /// </summary>
    /// <param name="guildSetting">Guild setting DTO</param>
    /// <returns>Created guild setting</returns>
    /// <response code="201">Returns created guild setting</response>
    [ProducesResponseType(typeof(GuildSettingDto), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGuildSettingDto guildSetting)
    {
        var createdGuildSetting = await _guildSettingFacade.CreateAsync(guildSetting);
        return CreatedAtAction(nameof(FindById), new {id = createdGuildSetting.Id}, createdGuildSetting);
    }
    
    // PUT: update guild setting
    /// <summary>
    ///     Updates guild setting
    /// </summary>
    /// <param name="id">Id of guild setting</param>
    /// <param name="guildSetting">Guild setting DTO</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="400">If guild setting is not valid or if ID is not a valid ObjectId</response>
    /// <response code="404">If guild setting is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateGuildSettingDto guildSetting)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildSettingFacade.UpdateAsync(id, guildSetting);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // DELETE: delete guild setting
    /// <summary>
    ///     Deletes guild setting
    /// </summary>
    /// <param name="id">Id of guild setting</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="400">If guild setting is not valid or if ID is not a valid ObjectId</response>
    /// <response code="404">If guild setting is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildSettingFacade.DeleteAsync(id);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

}