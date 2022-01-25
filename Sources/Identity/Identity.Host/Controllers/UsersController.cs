using System.Diagnostics.CodeAnalysis;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Shared.Features.Users;
using Shared.Persistence.Core.Features.Exceptions;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Identity.Features.Users.Single;

namespace Identity.Host.Controllers;

[Authorize(IdentityServerConstants.LocalApi.PolicyName, Roles = "user,admin")]
[Route("[controller]")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
#pragma warning disable CS1591
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserFacade              _usersOperationFacade;

    public UsersController(IUserFacade usersOperationFacade, ILogger<UsersController> logger)
#pragma warning restore CS1591
    {
        _usersOperationFacade = usersOperationFacade;
        _logger               = logger;
    }
    
    /// <summary>
    ///     Get current user profile
    /// </summary>
    /// <returns>Current user details</returns>
    /// <response code="200">Current user details</response>
    /// <response code="404">User not found</response>
    [ProducesResponseType(typeof(UserDto), 200)]
    [HttpGet("me", Name = nameof(Profile))]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userName = User.Claims.Single(x => x.Type == JwtClaimTypes.Name).Value;

        var user = await _usersOperationFacade.GetByUsernameAsync(userName);
        
        if (user is null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    ///     Get user by id
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>User with given ID</returns>
    /// <response code="200">User with given ID</response>
    /// <response code="404">User not found</response>
    [ProducesResponseType(typeof(UserDto), 200)]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await _usersOperationFacade.GetByIdAsync(id);
        if (user is null) return NotFound();
        return Ok(user);
    }
    
    /// <summary>
    ///     Create new user
    /// </summary>
    /// <param name="dto">User details DTO</param>
    /// <returns>Created user details</returns>
    /// <response code="201">Created user details</response>
    /// <response code="400"> If user details are invalid</response>
    [ProducesResponseType(typeof(UserDto), 201)]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = await _usersOperationFacade.CreateAsync(dto);
            return CreatedAtAction(nameof(Post), user);
        }
        catch (CreateUserException e)
        {
            _logger.LogDebug("Cannot create user because [{ErrorDescriptions}]", e.Message);
            return BadRequest(new
            {
                Errors = e.Causes,
            });
        }
    }

    /// <summary>
    ///     Get user by username
    /// </summary>
    /// <param name="username">User username</param>
    /// <returns>User with given username</returns>
    /// <response code="200">User with given username</response>
    /// <response code="404">User not found</response>
    [ProducesResponseType(typeof(UserDto), 200)]
    [HttpGet("UserName/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var user = await _usersOperationFacade.GetByUsernameAsync(username);
        if (user is null) return NotFound();
        return Ok(user);
    }


    /// <summary>
    ///     Update user
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="dto">User details DTO</param>
    /// <returns>No content</returns>
    /// <response code="204">Update successful</response>
    /// <response code="400"> If user details or ID are invalid</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
                return BadRequest("Id is not a valid Object ID!");
            
            await _usersOperationFacade.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>
    ///     Delete user
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>No content</returns>
    /// <response code="204">Delete successful</response>
    /// <response code="404">User not found</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
                return BadRequest("Id is not a valid Object ID!");
            
            await _usersOperationFacade.DeleteAsync(id);
            return NoContent();
        }
        catch (DocumentNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}