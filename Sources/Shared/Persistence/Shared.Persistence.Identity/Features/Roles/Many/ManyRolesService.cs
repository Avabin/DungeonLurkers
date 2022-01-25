using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Shared.Features.Roles;

namespace Shared.Persistence.Identity.Features.Roles.Many;

public class ManyRolesService : ManyDocumentServiceBase<RoleDocument, FindRoleDto>, IManyRolesService
{
    private readonly RoleManager<RoleDocument> _repository;

    public ManyRolesService(RoleManager<RoleDocument> repository, IMapper mapper) : base(mapper) =>
        _repository = repository;

    public Task<IEnumerable<FindRoleDto>> GetAllAsync(int? skip, int? limit)
    {
        var roles = GetAll(skip, limit, _repository.Roles);

        return Task.FromResult(roles);
    }

    public Task<IEnumerable<FindRoleDto>> GetAllByPredicateAsync(
        Expression<Func<RoleDocument, bool>> predicate,
        int?                                 skip,
        int?                                 limit)
    {
        var roles = GetAllByPredicate(predicate, skip, limit, _repository.Roles);
        return Task.FromResult(roles);
    }
}