using Shared.Features.Roles;
using Shared.Persistence.Core.Features.Documents.Single;

namespace Shared.Persistence.Identity.Features.Roles.Single;

public interface ISingleRoleService : ISingleDocumentService<RoleDocument, string, FindRoleDto>
{
    Task<FindRoleDto> GetByNameAsync(string name);
}