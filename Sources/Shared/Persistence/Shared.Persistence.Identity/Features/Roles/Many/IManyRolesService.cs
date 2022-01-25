using Shared.Features.Roles;
using Shared.Persistence.Core.Features.Documents.Many;

namespace Shared.Persistence.Identity.Features.Roles.Many;

public interface IManyRolesService : IManyDocumentsService<RoleDocument, string, FindRoleDto>
{
}