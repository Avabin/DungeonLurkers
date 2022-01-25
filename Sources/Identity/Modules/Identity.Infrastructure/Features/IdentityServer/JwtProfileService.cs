using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.Persistence.Identity.Features.Users;

namespace Identity.Infrastructure.Features.IdentityServer;

public class JwtProfileService : IProfileService
{
    private readonly IUserClaimsPrincipalFactory<UserDocument> _principalFactory;
    private readonly ILogger<JwtProfileService>                _logger;
    private readonly UserManager<UserDocument>                 _userManager;

    public JwtProfileService(
        UserManager<UserDocument>                 userManager,
        IUserClaimsPrincipalFactory<UserDocument> principalFactory,
        ILogger<JwtProfileService> logger)
    {
        _userManager      = userManager;
        _principalFactory = principalFactory;
        _logger      = logger;
    }
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub       = context.Subject.GetSubjectId();
        var user      = await _userManager.FindByIdAsync(sub);
        var principal = await _principalFactory.CreateAsync(user);

        var claims = principal.Claims.ToList();
        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
        claims.Add(new Claim(JwtClaimTypes.Name, user.UserName));

        user.Roles.ForEach(x => claims.Add(new Claim(JwtClaimTypes.Role, x)));

        claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var sub  = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(sub);
        var isActive = user != null;
        _logger.LogTrace("User with ID {UserId} is active: {IsActive}.", sub, isActive);
        context.IsActive = isActive;
    }
}