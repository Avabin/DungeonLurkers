using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TheDungeonGuide.Persistence.Sessions;
using TheDungeonGuide.Persistence.Sessions.Features.Exceptions;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Sessions.Host.Controllers;

[ApiController]
[Authorize(Roles = "player,gm,admin")]
[Route("/")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
public class SessionController : ControllerBase
{
    private readonly ISessionFacade _sessionFacade;

    public SessionController(ISessionFacade sessionFacade) => _sessionFacade = sessionFacade;

    /// <summary>
    ///     Fetches all sessions
    /// </summary>
    /// <param name="skip">Count of objects to skip from start</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of all sessions</returns>
    /// <response code="200">Returns all sessions</response>
    [ProducesResponseType(typeof(IEnumerable<SessionDto>), StatusCodes.Status200OK)]
    [HttpGet(Name = "GetAll")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Get(int skip = 0, int limit = 10)
    {
        var sessions = await _sessionFacade.GetAllAsync(skip, limit);
        return Ok(sessions);
    }

    /// <summary>
    ///     Fetches session by session ID
    /// </summary>
    /// <param name="id">Session ID</param>
    /// <returns>Found session object</returns>
    /// <response code="200">Returns session object</response>
    /// <response code="404">Session not found</response>
    /// <response code="400">Invalid session ID</response>
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSessionById([FromRoute] string id)
    {

        if (!ObjectId.TryParse(id, out _)) return BadRequest("Id is not a valid ObjectId");

        var session = await _sessionFacade.GetByIdAsync(id);
        
        if (session is null) return NotFound();
        return Ok(session);
    }

    /// <summary>
    ///     Fetches all sessions that have given user as game master
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="skip">Count of objects to skip</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of sessions that has given user as game master</returns>
    /// <response code="200">Returns list of sessions that has given user as game master</response>
    /// <response code="400">If the game master ID is not a valid ObjectId</response>
    /// <response code="404">If the game master is not found</response>
    [ProducesResponseType(typeof(IEnumerable<SessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("gm/{id}", Name = "GetByGameMasterId")]
    [Authorize(Roles = "gm")]
    public async Task<IActionResult> GetSessionsByGameMasterId(string id, int skip = 0, int limit = 10)
    {
        var sessions = await _sessionFacade.GetAllByGameMasterIdAsync(id, skip, limit);
        return Ok(sessions);
    }

    /// <summary>
    ///     Fetches all sessions that have a character with the given id
    /// </summary>
    /// <param name="id">Character ID</param>
    /// <param name="skip">Count of objects to skip</param>
    /// <param name="limit">Count of objects to take</param>
    /// <returns>List of sessions that have given character as member</returns>
    /// <response code="200">Returns the list of sessions for a given character</response>
    /// <response code="400">If the character id is not valid</response>
    /// <response code="404">If session is not found</response>
    [ProducesResponseType(typeof(IEnumerable<SessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("characters/{id}", Name = "GetByCharacterId")]
    public async Task<IActionResult> GetSessionsByCharacterId(string id, int? skip = 0, int? limit = 10)
    {
        var sessions = await _sessionFacade.GetAllByCharacterIdAsync(id, skip, limit);
        return Ok(sessions);
    }

    /// <summary>
    ///     Removes a player from a session
    /// </summary>
    /// <param name="id">Session id</param>
    /// <param name="playerId">Player id</param>
    /// <returns>NoContent if success</returns>
    /// <response code="204">Player removed from session</response>
    /// <response code="404">Session or player not found in session</response>
    /// <response code="400">Session id or player id is not a valid ObjectId</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id}/players/{playerId}", Name = nameof(RemovePlayer))]
    [Authorize(Roles = "admin,gm")]
    public async Task<IActionResult> RemovePlayer(string id, string playerId)
    {
        try
        {
            await _sessionFacade.RemovePlayerAsync(id, playerId);
            return NoContent();
        }
        catch (MemberNotFoundException)
        {
            return NotFound("Member not found in session");
        }
        catch (SessionNotFoundException)
        {
            return NotFound("Session not found!");
        }
    }

    /// <summary>
    ///     Deletes a character from a session
    /// </summary>
    /// <param name="id">Session ID</param>
    /// <param name="characterId">Character ID</param>
    /// <returns>NoContent</returns>
    /// <response code="204">Character deleted from session</response>
    /// <response code="400">Session ID or character ID is not a valid ObjectId</response>
    /// <response code="404">Session not found, or character not found in session</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [HttpDelete("{id}/characters/{characterId}", Name = nameof(RemoveCharacter))]
    [Authorize(Roles = "admin,gm")]
    public async Task<IActionResult> RemoveCharacter(string id, string characterId)
    {
        try
        {
            await _sessionFacade.RemoveCharacterAsync(id, characterId);
            return NoContent();
        }
        catch (CharacterNotFoundException)
        {
            return NotFound("Character not found in session");
        }
        catch (SessionNotFoundException)
        {
            return NotFound("Session not found!");
        }
    }

    /// <summary>
    ///     Creates a new session from the given session dto.
    /// </summary>
    /// <param name="sessionDto">Session definition</param>
    /// <returns>Newly created session</returns>
    /// <response code="201">Returns the newly created session</response>
    /// <response code="400">If request model is invalid, returns error details</response>
    [HttpPost(Name = nameof(CreateSession))]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "admin,gm")]
    public async Task<IActionResult> CreateSession(CreateSessionDto sessionDto)
    {
        var session = await _sessionFacade.CreateAsync(sessionDto);
        return CreatedAtAction(nameof(GetSessionById), new
        {
            id = session.Id,
        }, session);
    }
}