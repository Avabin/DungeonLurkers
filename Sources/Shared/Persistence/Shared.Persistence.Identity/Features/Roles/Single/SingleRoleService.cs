using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Shared.Features.Roles;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Identity.Features.Roles.Single;

public class SingleRoleService : SingleDocumentService<RoleDocument, string, FindRoleDto>, ISingleRoleService
{
    private readonly IMapper                   _mapper;
    private readonly RoleManager<RoleDocument> _roleManager;

    public SingleRoleService(
        RoleManager<RoleDocument>         roleManager,
        IMapper                           mapper,
        IRepository<RoleDocument, string> repository) :
        base(repository, mapper)
    {
        _roleManager = roleManager;
        _mapper      = mapper;
    }

    public async Task<FindRoleDto> GetByNameAsync(string name)
    {
        var roleDocument = await _roleManager.FindByNameAsync(name);
        return _mapper.Map<FindRoleDto>(roleDocument);
    }

    public new Task UpdateAsync<TUpdateDto>(string id, TUpdateDto dto)
    {
        var doc = _mapper.Map<RoleDocument>(dto);
        doc.Id = id;
        return _roleManager.UpdateAsync(doc);
    }

    public new async Task<FindRoleDto> DeleteAsync(string id)
    {
        var existing = await _roleManager.FindByIdAsync(id);
        await _roleManager.DeleteAsync(existing);
        return _mapper.Map<FindRoleDto>(existing);
    }

    public new async Task<FindRoleDto> CreateAsync<TCreateDocDto>(TCreateDocDto request)
    {
        var doc = _mapper.Map<RoleDocument>(request);
        await _roleManager.CreateAsync(doc);
        doc = await _roleManager.FindByNameAsync(doc.Name);
        return _mapper.Map<FindRoleDto>(doc);
    }

    public new async Task<FindRoleDto> GetByIdAsync(string id)
    {
        var doc = await _roleManager.FindByIdAsync(id);
        return _mapper.Map<FindRoleDto>(doc);
    }

    public new Task<FindRoleDto> GetByPredicateAsync(Expression<Func<RoleDocument, bool>> predicate)
    {
        var doc = _roleManager.Roles.Single(predicate);

        var mapped = _mapper.Map<FindRoleDto>(doc);
        return Task.FromResult(mapped);
    }
}