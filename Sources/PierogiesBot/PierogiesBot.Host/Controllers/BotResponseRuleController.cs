using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PierogiesBot.Persistence.BotResponseRules.Features;
using PierogiesBot.Shared.Features.BotResponseRules;
using Shared.Persistence.Core.Features.Exceptions;

namespace PierogiesBot.Host.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
#pragma warning disable CS1591
public class BotResponseRuleController : ControllerBase
{
    private readonly IBotResponseRuleFacade _botResponseRuleFacade;
    
    public BotResponseRuleController(IBotResponseRuleFacade botResponseRuleFacade)
#pragma warning restore CS1591
    {
        _botResponseRuleFacade = botResponseRuleFacade;
    }
    
    /// <summary>
    ///     Get all bot response rules
    /// </summary>
    /// <param name="skip">Count of objects to skip from start</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of bot response rules</returns>
    /// <response code="200">Returns the list of bot response rules</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BotResponseRuleDto>), 200)]
    public async Task<IActionResult> GetAll(int? skip, int? limit)
    {
        var botResponseRules = await _botResponseRuleFacade.GetAllAsync(skip, limit);
        return Ok(botResponseRules);
    }
    
    /// <summary>
    ///     Get bot response rule by ID
    /// </summary>
    /// <param name="id">ID of bot response rule</param>
    /// <returns>Bot response rule</returns>
    /// <response code="200">Returns the bot response rule</response>
    /// <response code="404">If the bot response rule is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BotResponseRuleDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(string id)
    {
        if(!ObjectId.TryParse(id, out _))
            return BadRequest();

        var botResponseRule = await _botResponseRuleFacade.GetByIdAsync(id);
        if (botResponseRule == null)
        {
            return NotFound();
        }

        return Ok(botResponseRule);
    }
    
    /// <summary>
    ///     Create new bot response rule
    /// </summary>
    /// <param name="dto">Bot response rule DTO</param>
    /// <returns>Created bot response rule</returns>
    /// <response code="201">Returns the created bot response rule</response>
    /// <response code="400">If the bot response rule is not valid</response>
    [HttpPost]
    [ProducesResponseType(typeof(BotResponseRuleDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateBotResponseRuleDto dto)
    {
        var botResponseRule = await _botResponseRuleFacade.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new {id = botResponseRule.Id}, botResponseRule);
    }
    
    /// <summary>
    ///     Update bot response rule
    /// </summary>
    /// <param name="id">ID of bot response rule</param>
    /// <param name="dto">Bot response rule update DTO</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="400">If the bot response rule is not valid</response>
    /// <response code="404">If the bot response rule is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBotResponseRuleDto dto)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest();

        try
        {
            await _botResponseRuleFacade.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    /// <summary>
    ///     Delete bot response rule
    /// </summary>
    /// <param name="id">ID of bot response rule</param>
    /// <returns>No content</returns>
    /// <response code="204">Returns no content</response>
    /// <response code="404">If the bot response rule is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest();

        try
        {
            await _botResponseRuleFacade.DeleteAsync(id);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

}