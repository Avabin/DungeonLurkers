using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Shared.Features.BotCrontabRules;
using Shared.Persistence.Core.Features.Exceptions;

namespace PierogiesBot.Host.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
#pragma warning disable CS1591
public class BotCrontabRuleController : ControllerBase
{
    private readonly IBotCrontabRuleFacade _botCrontabRuleFacade;
    
    public BotCrontabRuleController(IBotCrontabRuleFacade botCrontabRuleFacade)
#pragma warning restore CS1591
    {
        _botCrontabRuleFacade = botCrontabRuleFacade;
    }
    
    /// <summary>
    ///     Fetches all Crontab rules
    /// </summary>
    /// <param name="skip">Count of objects to skip from start</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of all crontab rules</returns>
    /// <response code="200">Returns all crontab rules</response>
    [ProducesResponseType(typeof(IEnumerable<BotCrontabRuleDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> All(int? skip, int? limit) => 
        Ok(await _botCrontabRuleFacade.GetAllAsync(skip, limit));
    
    
    /// <summary>
    ///     Fetches Crontab rule by id
    /// </summary>
    /// <param name="id">Id of the Crontab rule</param>
    /// <returns>Crontab rule</returns>
    /// <response code="200">Returns Crontab rule</response>
    /// <response code="404">If rule with given ID was not found</response>
    [ProducesResponseType(typeof(BotCrontabRuleDto), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid id");

        var result = await _botCrontabRuleFacade.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    /// <summary>
    ///     Creates new BotCrontabRule
    /// </summary>
    /// <param name="botCrontabRuleDto">Crontab rule to create</param>
    /// <returns>Created BotCrontabRule</returns>
    /// <response code="201">Returns created BotCrontabRule</response>
    /// <response code="400">If validation failed</response>
    [ProducesResponseType(typeof(BotCrontabRuleDto), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBotCrontabRuleDto botCrontabRuleDto)
    {
        var result = await _botCrontabRuleFacade.CreateAsync(botCrontabRuleDto);
        return CreatedAtAction(nameof(Create), new {id = result.Id}, result);
    }
    
    /// <summary>
    ///     Updates existing BotCrontabRule
    /// </summary>
    /// <param name="id">Id of the BotCrontabRule</param>
    /// <param name="botCrontabRuleDto">DTO of updated BotCrontabRule</param>
    /// <returns>No content</returns>
    /// <response code="204">Update successful</response>
    /// <response code="400">If ID is invalid</response>
    /// <response code="404">If rule with given ID was not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBotCrontabRuleDto botCrontabRuleDto)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid id");

        try
        {
            await _botCrontabRuleFacade.UpdateAsync(id, botCrontabRuleDto);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    /// <summary>
    ///     Add response to scheduled message rule
    /// </summary>
    /// <param name="id">ID of bot scheduled message rule</param>
    /// <param name="response">Response value to add</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="404">If the bot scheduled message rule is not found</response>
    [HttpPost("{id}/responses")]
    public async Task<IActionResult> AddResponseToRule(string id, string response)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest();

        try
        {
            await _botCrontabRuleFacade.AddResponseToRuleAsync(id, response);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>
    ///     Delete response from scheduled message rule
    /// </summary>
    /// <param name="id">ID of bot scheduled message rule</param>
    /// <param name="response">Response value to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="404">If the bot scheduled message rule is not found</response>
    [HttpDelete("{id}/responses")]
    public async Task<IActionResult> RemoveResponseFromRule(string id, string response)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest();

        try
        {
            await _botCrontabRuleFacade.RemoveResponseFromRuleAsync(id, response);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    /// <summary>
    ///     Add emote to scheduled message rule
    /// </summary>
    /// <param name="id">ID of bot scheduled message rule</param>
    /// <param name="emote">Emote value to add</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="404">If the bot scheduled message rule is not found</response>
    [HttpPost("{id}/emotes")]
    public async Task<IActionResult> AddEmoteToRule(string id, string emote)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest();

        try
        {
            await _botCrontabRuleFacade.AddEmojiToRuleAsync(id, emote);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>
    ///     Delete emote from scheduled message rule
    /// </summary>
    /// <param name="id">ID of bot scheduled message rule</param>
    /// <param name="emote">Emote value to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="404">If the bot scheduled message rule is not found</response>
    [HttpDelete("{id}/emotes")]
    public async Task<IActionResult> RemoveEmoteFromRule(string id, string emote)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest();

        try
        {
            await _botCrontabRuleFacade.RemoveEmojiFromRuleAsync(id, emote);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    /// <summary>
    ///     Deletes existing scheduled message rule
    /// </summary>
    /// <param name="id">Id of the scheduled message rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Delete successful</response>
    /// <response code="400">If ID is invalid</response>
    /// <response code="404">If rule with given ID was not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid id");

        try
        {
            await _botCrontabRuleFacade.DeleteAsync(id);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}