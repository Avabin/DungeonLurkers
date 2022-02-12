using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PierogiesBot.Persistence.BotReactRules.Features;
using PierogiesBot.Shared.Features.BotReactRules;
using RestEase;
using Shared.Persistence.Core.Features.Exceptions;

namespace PierogiesBot.Host.Controllers;
#pragma warning disable CS1591
[Route("[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
public class BotReactRuleController : ControllerBase
{
    private readonly IBotReactRuleFacade _facade;

    public BotReactRuleController(IBotReactRuleFacade facade) => _facade = facade;
#pragma warning restore CS1591

    /// <summary>
    ///     Fetches all reaction rules
    /// </summary>
    /// <param name="skip">Count of objects to skip from start</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of all characters</returns>
    /// <response code="200">Returns all characters</response>
    [ProducesResponseType(typeof(IEnumerable<BotReactionRuleDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> All(int? skip, int? limit)
    {
        var result = await _facade.GetAllAsync(skip, limit);
        return Ok(result);
    }
    
    /// <summary>
    ///     Fetches reaction rule by its ID
    /// </summary>
    /// <param name="id">Reaction rule ID</param>
    /// <returns>Found reaction rule object</returns>
    /// <response code="200">Returns reaction rule object</response>
    /// <response code="404">Reaction rule not found</response>
    /// <response code="400">Invalid reaction rule ID</response>
    [ProducesResponseType(typeof(BotReactionRuleDto), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<IActionResult> FindById(string id)
    {
        if(!ObjectId.TryParse(id, out _)) return BadRequest("Invalid id");
        var result = await _facade.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }
    
    /// <summary>
    ///     Creates a new reaction rule
    /// </summary>
    /// <param name="dto">Reaction rule DTO</param>
    /// <returns>Created reaction rule</returns>
    /// <response code="201">Returns newly created reaction rule</response>
    /// <response code="400">Invalid reaction rule DTO</response>
    [ProducesResponseType(typeof(BotReactionRuleDto), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBotReactionRuleDto dto)
    {
        var result = await _facade.CreateAsync(dto);
        return CreatedAtAction(nameof(Create), new {id = result.Id}, result);
    }
    
    /// <summary>
    ///     Updates a specified reaction rule
    /// </summary>
    /// <param name="id">Rule ID</param>
    /// <param name="dto">Updated reaction rule object DTO</param>
    /// <returns>204 if success</returns>
    /// <response code="204">Rule updated</response>
    /// <response code="400">Invalid DTO</response>
    /// <response code="400">Invalid rule ID</response>
    /// <response code="404">Rule not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBotReactionRuleDto dto)
    {
        if(!ObjectId.TryParse(id, out _)) return BadRequest("Invalid id");

        try
        {
            await _facade.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    /// <summary>
    ///     Add reaction to rule
    /// </summary>
    /// <param name="id">ID of bot reaction rule</param>
    /// <param name="response">Reaction value to add</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="404">If the bot reaction rule is not found</response>
    [HttpPost("{id}/Reactions")]
    public async Task<IActionResult> AddReactionToRule(string id, string response)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest();

        try
        {
            await _facade.AddReactionToRuleAsync(id, response);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>
    ///     Delete reaction from rule
    /// </summary>
    /// <param name="id">ID of bot response rule</param>
    /// <param name="response">Response value to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="404">If the bot response rule is not found</response>
    [HttpDelete("{id}/reactions")]
    public async Task<IActionResult> RemoveReactionFromRule(string id, string response)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest();

        try
        {
            await _facade.RemoveReactionFromRuleAsync(id, response);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    /// <summary>
    ///     Deletes a specified reaction rule
    /// </summary>
    /// <param name="id">Bot react rule ID</param>
    /// <returns>204 if success</returns>
    /// <response code="204">Rule deleted</response>
    /// <response code="404">Rule not found</response>
    /// <response code="400">Bad request, ID is invalid</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if(!ObjectId.TryParse(id, out _)) return BadRequest("Invalid id");

        try
        {
            await _facade.DeleteAsync(id);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}