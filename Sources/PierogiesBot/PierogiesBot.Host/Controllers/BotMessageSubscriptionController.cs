using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PierogiesBot.Persistence.BotMessageSubscription.Features;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using Shared.Persistence.Core.Features.Exceptions;

namespace PierogiesBot.Host.Controllers;

#pragma warning disable CS1591
[Route("[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
public class BotMessageSubscriptionController : ControllerBase
{
    private readonly IBotMessageSubscriptionFacade _botMessageSubscriptionFacade;
    
    public BotMessageSubscriptionController(IBotMessageSubscriptionFacade botMessageSubscriptionFacade)
#pragma warning restore CS1591
    {
        _botMessageSubscriptionFacade = botMessageSubscriptionFacade;
    }
    
    /// <summary>
    ///     Fetches all messages subscription rules
    /// </summary>
    /// <param name="skip">Count of objects to skip from start</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of all message subscription rules</returns>
    /// <response code="200">Returns all message subscription rules</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<BotMessageSubscriptionDto>), 200)]
    public async Task<IActionResult> All(int? skip, int? limit)
    {
        var result = await _botMessageSubscriptionFacade.GetAllAsync(skip, limit);
        return Ok(result);
    }
    
    /// <summary>
    ///     Fetches all messages subscription rules for specific guild channel
    /// </summary>
    /// <param name="guildId">Guild id</param>
    /// <param name="channelId">Channel id</param>
    /// <returns>List of all message subscription rules for channel</returns>
    /// <response code="200">Returns all message subscription rules for channel</response>
    [HttpGet("guild/{guildId}/channel/{channelId}")]
    [ProducesResponseType(typeof(List<BotMessageSubscriptionDto>), 200)]
    public async Task<IActionResult> GetAllForChannel(ulong guildId, ulong channelId)
    {
        var result = await _botMessageSubscriptionFacade.GetAllSubscriptionsForChannelAsync(channelId, guildId);
        return Ok(result);
    }
    
    // GET: Get all subscriptions for guild
    /// <summary>
    ///     Fetches all messages subscription rules for specific guild
    /// </summary>
    /// <param name="guildId">Guild id</param>
    /// <returns>List of all message subscription rules for guild</returns>
    /// <response code="200">Returns all message subscription rules for guild</response>
    [HttpGet("guild/{guildId}")]
    [ProducesResponseType(typeof(List<BotMessageSubscriptionDto>), 200)]
    public async Task<IActionResult> GetAllForGuild(ulong guildId)
    {
        var result = await _botMessageSubscriptionFacade.GetAllSubscriptionsForGuildAsync(guildId);
        return Ok(result);
    }

    /// <summary>
    ///     Finds message subscription rule by id
    /// </summary>
    /// <param name="id">Id of message subscription rule</param>
    /// <returns>Message subscription rule</returns>
    /// <response code="200">Returns message subscription rule</response>
    /// <response code="404">If message subscription rule was not found</response>
    /// <response code="400">If message subscription rule id is invalid</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BotMessageSubscriptionDto), 200)]
    public async Task<IActionResult> FindById(string id)
    {
        if(!ObjectId.TryParse(id, out _)) return BadRequest("Invalid id");
        var result = await _botMessageSubscriptionFacade.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }
    
    /// <summary>
    ///     Creates new message subscription rule
    /// </summary>
    /// <param name="dto">Message subscription rule</param>
    /// <returns>Created message subscription rule</returns>
    /// <response code="201">Returns created message subscription rule</response>
    /// <response code="400">If message subscription rule is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(BotMessageSubscriptionDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateBotMessageSubscriptionDto dto)
    {
        var result = await _botMessageSubscriptionFacade.CreateAsync(dto);
        return CreatedAtAction(nameof(Create), new {id = result.Id}, result);
    }
    
    /// <summary>
    ///     Updates message subscription rule
    /// </summary>
    /// <param name="id">Id of message subscription rule</param>
    /// <param name="dto">Message subscription rule</param>
    /// <returns>No content</returns>
    /// <response code="200">Update successful</response>
    /// <response code="404">If message subscription rule was not found</response>
    /// <response code="400">If updated message subscription rule is invalid</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BotMessageSubscriptionDto), 200)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBotMessageSubscriptionDto dto)
    {
        if (!ObjectId.TryParse(id, out _)) return BadRequest("Invalid id");

        try
        {
            await _botMessageSubscriptionFacade.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    /// <summary>
    ///    Deletes message subscription rule
    /// </summary>
    /// <param name="id">Id of message subscription rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Delete successful</response>
    /// <response code="404">If message subscription rule was not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return BadRequest("Invalid id");
        try
        {
            await _botMessageSubscriptionFacade.DeleteAsync(id);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

}