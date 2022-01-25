using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.Features.Users;

namespace Shared.Persistence.Identity.Features.Users.Many;

public class ManyUsersService : ManyDocumentServiceBase<UserDocument, UserDto>, IManyUsersService
{
    private readonly ILogger<ManyUsersService> _logger;
    private readonly UserManager<UserDocument> _userManager;

    public ManyUsersService(
        UserManager<UserDocument> userManager,
        IMapper                   mapper,
        ILogger<ManyUsersService> logger) : base(mapper)
    {
        _userManager = userManager;
        _logger      = logger;
    }
    public Task<IEnumerable<UserDto>> GetAllAsync(int? skip, int? limit)
    {
        _logger.LogTrace("{Action}({Skip}, {Limit}", nameof(GetAllAsync), skip ?? 0, limit ?? 0);
        var users = GetAll(skip, limit, _userManager.Users);

        return Task.FromResult(users);
    }

    public Task<IEnumerable<UserDto>> GetAllByPredicateAsync(
        Expression<Func<UserDocument, bool>> predicate,
        int?                                 skip,
        int?                                 limit)
    {
        _logger.LogTrace("{Action}({Skip}, {Limit}", nameof(GetAllByPredicateAsync), skip ?? 0, limit ?? 0);
        var users = GetAllByPredicate(predicate, skip, limit, _userManager.Users);

        return Task.FromResult(users);
    }
}