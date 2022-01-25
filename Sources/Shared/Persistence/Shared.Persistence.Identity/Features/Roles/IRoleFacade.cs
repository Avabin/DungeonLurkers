using Shared.Persistence.Identity.Features.Roles.Many;
using Shared.Persistence.Identity.Features.Roles.Single;

namespace Shared.Persistence.Identity.Features.Roles;

public interface IRoleFacade : ISingleRoleService, IManyRolesService
{
}