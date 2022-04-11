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
        if(!ObjectId.TryParse(id, out _))
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
    
    // GET: get all subscribed channels
    /// <summary>
    /// Get all subscribed channels
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <returns>List of subscribed channels</returns>
    /// <response code="200">Returns the list of subscribed channels</response>
    /// <response code="404">Guild not found</response>
    [Authorize(Roles = "user")]
    [HttpGet("{id}/channels")]
    [ProducesResponseType(typeof(List<GuildSubscribedChannelsDto>), 200)]
    public async Task<IActionResult> GetSubscribedChannels(string id)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            var channels = await _guildFacade.GetAllSubscribedChannelsAsync(id);
            
            return Ok(channels);
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    
    // PUT: Add subscribed channel ID (ulong) to guild
    /// <summary>
    /// Add subscribed channel ID (ulong) to guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="channelId">Id of the channel</param>
    /// <returns>No content</returns>
    /// <response code="204">Channel added to guild</response>
    /// <response code="404">Guild or channel not found</response>
    [HttpPut("{id}/channels")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AddChannel(string id, ulong channelId)
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
    
    // DELETE: Unsubscribe channel from guild
    /// <summary>
    /// Unsubscribe channel from guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="channelId">Id of the channel</param>
    /// <returns>No content</returns>
    /// <response code="204">Channel unsubscribed</response>
    /// <response code="404">Guild or channel not found</response>
    [HttpDelete("{id}/channels")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveChannel(string id, ulong channelId)
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
    
    // GET: get all subscribed crontab rules
    /// <summary>
    /// Get all subscribed crontab rules
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <returns>List of subscribed crontab rules</returns>
    /// <response code="200">Returns the list of subscribed crontab rules</response>
    /// <response code="404">Guild not found</response>
    [Authorize(Roles = "user")]
    [ProducesResponseType(typeof(GuildSubscribedRulesDto), 200)]
    [HttpGet("{id}/crontab")]
    public async Task<IActionResult> GetSubscribedCrontabRules(string id)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            var rules = await _guildFacade.GetAllSubscribedCrontabRulesAsync(id);
            
            return Ok(rules);
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // PUT: Add subscribed crontab rule ID (string) to guild
    /// <summary>
    /// Add subscribed crontab rule ID (string) to guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="ruleId">Id of the crontab rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Crontab rule added to guild</response>
    /// <response code="404">Guild or crontab rule not found</response>
    [HttpPut("{id}/crontab")]
    public async Task<IActionResult> AddCrontabRule(string id, string ruleId)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.EnableCrontabRuleAsync(id, ruleId);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // DELETE: Unsubscribe crontab rule from guild
    /// <summary>
    /// Unsubscribe crontab rule from guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="ruleId">Id of the crontab rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Crontab rule unsubscribed</response>
    /// <response code="404">Guild or crontab rule not found</response>
    [HttpDelete("{id}/crontab")]
    public async Task<IActionResult> RemoveCrontabRule(string id, string ruleId)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.DisableCrontabRuleAsync(id, ruleId);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // GET: Get all subscribed response rules
    /// <summary>
    /// Get all subscribed response rules
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <returns>List of subscribed response rules</returns>
    /// <response code="200">Returns the list of subscribed response rules</response>
    /// <response code="404">Guild not found</response>
    [Authorize(Roles = "user")]
    [ProducesResponseType(typeof(GuildSubscribedRulesDto), 200)]
    [HttpGet("{id}/responses")]
    public async Task<IActionResult> GetSubscribedResponseRules(string id)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            var rules = await _guildFacade.GetAllSubscribedResponseRulesAsync(id);
            
            return Ok(rules);
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // PUT: Add subscribed response rule ID (string) to guild
    /// <summary>
    /// Add subscribed response rule ID (string) to guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="ruleId">Id of the response rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Response rule added to guild</response>
    /// <response code="404">Guild or response rule not found</response>
    [HttpPut("{id}/responses")]
    public async Task<IActionResult> AddResponseRule(string id, string ruleId)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.EnableResponseRuleAsync(id, ruleId);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // DELETE: Unsubscribe response rule from guild
    /// <summary>
    /// Unsubscribe response rule from guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="ruleId">Id of the response rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Response rule unsubscribed</response>
    /// <response code="404">Guild or response rule not found</response>
    [HttpDelete("{id}/responses")]
    public async Task<IActionResult> RemoveResponseRule(string id, string ruleId)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.DisableResponseRuleAsync(id, ruleId);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // GET: Get all subscribed reaction rules
    /// <summary>
    /// Get all subscribed reaction rules
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <returns>List of subscribed reaction rules</returns>
    /// <response code="200">Returns the list of subscribed reaction rules</response>
    /// <response code="404">Guild not found</response>
    [Authorize(Roles = "user")]
    [ProducesResponseType(typeof(GuildSubscribedRulesDto), 200)]
    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetSubscribedReactionRules(string id)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            var rules = await _guildFacade.GetAllSubscribedReactionRulesAsync(id);
            
            return Ok(rules);
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // PUT: Add subscribed reaction rule ID (string) to guild
    /// <summary>
    /// Add subscribed reaction rule ID (string) to guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="ruleId">Id of the reaction rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Reaction rule added to guild</response>
    /// <response code="404">Guild or reaction rule not found</response>
    [HttpPut("{id}/reactions")]
    public async Task<IActionResult> AddReactionRule(string id, string ruleId)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.EnableReactionRuleAsync(id, ruleId);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    // DELETE: Unsubscribe reaction rule from guild
    /// <summary>
    /// Unsubscribe reaction rule from guild
    /// </summary>
    /// <param name="id">Id of the guild</param>
    /// <param name="ruleId">Id of the reaction rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Reaction rule unsubscribed</response>
    /// <response code="404">Guild or reaction rule not found</response>
    [HttpDelete("{id}/reactions")]
    public async Task<IActionResult> RemoveReactionRule(string id, string ruleId)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not valid");

        try
        {
            await _guildFacade.DisableReactionRuleAsync(id, ruleId);
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