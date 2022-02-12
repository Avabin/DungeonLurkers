using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PierogiesBot.Persistence.Guild.Features;
using PierogiesBot.Persistence.Guild.Features.Single;
using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Exceptions;

namespace PierogiesBot.Host.Controllers;

/// <summary>
///    Controller for managing discord guilds
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
public class GuildController : ControllerBase
{
    private readonly IGuildFacade _guildFacade;

    public GuildController(IGuildFacade guildFacade)
    {
        _guildFacade = guildFacade;
    }
    
    // GET: get all with nullable skip and limit
    /// <summary>
    /// Get all guilds
    /// </summary>
    /// <param name="skip">Number of guilds to skip</param>
    /// <param name="limit">Number of guilds to return</param>
    /// <returns>List of guilds</returns>
    /// <response code="200">Returns the list of guilds</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GuildDto>), 200)]
    public async Task<IActionResult> GetAll(int? skip, int? limit)
    {
        var guilds = await _guildFacade.GetAllAsync(skip, limit);
        return Ok(guilds);
    }
    
    // GET: get by id
    /// <summary>
    /// Get guild by id
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <returns>Guild</returns>
    /// <response code="200">Returns the guild</response>
    /// <response code="404">Guild not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GuildDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(string id)
    {
        if(!ObjectId.TryParse(id, out var objectId))
            return BadRequest("Id is not valid");

        var guild = await _guildFacade.GetByIdAsync(id);
        if (guild == null)
        {
            return NotFound();
        }

        return Ok(guild);
    }
    
    // POST: create
    /// <summary>
    /// Create a new guild
    /// </summary>
    /// <param name="guildDto">DTO defining guild to create</param>
    /// <returns>Guild</returns>
    /// <response code="201">Returns the created guild</response>
    [HttpPost]
    [ProducesResponseType(typeof(GuildDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateGuildDto guildDto)
    {
        var guild = await _guildFacade.CreateAsync(guildDto);
        return StatusCode(201, guild);
    }
    
    // PUT: update
    /// <summary>
    /// Update a guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="guildDto">DTO defining guild update</param>
    /// <returns>No content</returns>
    /// <response code="204">Guild updated</response>
    /// <response code="404">Guild not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateGuildDto guildDto)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.UpdateAsync(id, guildDto);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // PUT: Add channel ID (ulong) to guild
    /// <summary>
    /// Add channel ID (ulong) to guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="channelId">Id of the channel</param>
    /// <returns>No content</returns>
    /// <response code="204">Channel added to guild</response>
    /// <response code="404">Guild or channel not found</response>
    [HttpPut("{id}/channels")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AddChannel(string id, [FromBody] ulong channelId)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.AddSubscribedChannelAsync(id, channelId);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // DELETE: Remove channel from guild
    /// <summary>
    /// Remove channel from guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="channelId">Id of the channel</param>
    /// <returns>No content</returns>
    /// <response code="204">Channel removed from guild</response>
    /// <response code="404">Guild or channel not found</response>
    [HttpDelete("{id}/channels")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveChannel(string id, [FromBody] ulong channelId)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.RemoveSubscribedChannelAsync(id, channelId);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // DELETE: delete
    /// <summary>
    /// Delete a guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <returns>No content</returns>
    /// <response code="204">Guild deleted</response>
    /// <response code="404">Guild not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.DeleteAsync(id);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
}