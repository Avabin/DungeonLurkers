using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Shared.Persistence.Core.Features.Exceptions;
using TheDungeonGuide.Persistence.Characters;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Characters.Host.Controllers;

[Produces("application/json")]
[ApiController]
[Route("[controller]")]
[Authorize(Roles = "user,gm,admin")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
#pragma warning disable CS1591
public class CharacterController : ControllerBase
{
    private readonly ICharacterFacade _characterFacade;

    public CharacterController(ICharacterFacade characterFacade) => _characterFacade = characterFacade;
#pragma warning restore CS1591

    /// <summary>
    ///     Fetches all characters
    /// </summary>
    /// <param name="skip">Count of objects to skip from start</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of all characters</returns>
    /// <response code="200">Returns all characters</response>
    [ProducesResponseType(typeof(IEnumerable<CharacterDto>), StatusCodes.Status200OK)]
    [HttpGet(Name = nameof(All))]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> All(int? skip, int? limit)
    {
        var characters = await _characterFacade.GetAllAsync(skip, limit);
        return Ok(characters);
    }

    /// <summary>
    ///     Fetches character by character ID
    /// </summary>
    /// <param name="id">Character ID</param>
    /// <returns>Found character object</returns>
    /// <response code="200">Returns character object</response>
    /// <response code="404">Character not found</response>
    /// <response code="400">Invalid character ID</response>
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<IActionResult> FindById(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Id is not a valid Object ID!");
        
        var result = await _characterFacade.GetByIdAsync(id);
        
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    ///     Fetches a character by name
    /// </summary>
    /// <param name="name">Character name</param>
    /// <returns>Character object</returns>
    /// <response code="200">Returns character object</response>
    /// <response code="404">Character not found</response>
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status200OK)]
    [HttpGet("name/{name}")]
    public async Task<IActionResult> FindByName(string name)
    {
        var result = await _characterFacade.FindByNameAsync(name);
        
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    ///     Fetches all characters by owned by user with OwnerId
    /// </summary>
    /// <param name="ownerId">Owner user ID</param>
    /// <param name="skip">Skip by x elements</param>
    /// <param name="limit">Limit to y elements starting from x</param>
    /// <returns>List of found characters</returns>
    [ProducesResponseType(typeof(IEnumerable<CharacterDto>), StatusCodes.Status200OK)]
    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> FindByOwnerId(string ownerId, int? skip = null, int? limit = null) =>
        Ok(await _characterFacade.FindAllByOwnerIdAsync(ownerId, skip, limit));

    /// <summary>
    ///     Creates a new character
    /// </summary>
    /// <param name="character">Character definition</param>
    /// <returns>Created character</returns>
    /// <response code="201">Returns newly created character</response>
    /// <response code="400">Invalid character definition</response>
    [ProducesResponseType(typeof(CharacterDto), StatusCodes.Status201Created)]
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(CreateCharacterDto character)
    {
        var doc = await _characterFacade.CreateAsync(character);
        var id  = doc.Id;
        return CreatedAtAction(nameof(FindById), new { id }, doc);
    }

    /// <summary>
    ///     Updates a specified character
    /// </summary>
    /// <param name="id">Character ID</param>
    /// <param name="character">Updated character object</param>
    /// <returns>204 if success</returns>
    /// <response code="204">Character updated</response>
    /// <response code="400">Invalid DTO</response>
    /// <response code="400">Invalid character ID</response>
    /// <response code="404">Character not found</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(string id, UpdateCharacterDto character)
    {
        if (!ObjectId.TryParse(id, out _)) return BadRequest("Id is not a valid Object ID!");

        try
        {
            await _characterFacade.UpdateAsync(id, character);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>
    ///     Deletes a specified character
    /// </summary>
    /// <param name="id">Character ID</param>
    /// <returns>200 if success</returns>
    /// <response code="200">Character deleted</response>
    /// <response code="404">Character not found</response>
    /// <response code="400">Bad request, ID is invalid</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return BadRequest("Id is not a valid Object ID!");

        try
        {
            await _characterFacade.DeleteAsync(id);
            return Ok();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}