using Shared.Features;
using Shared.Features.Roles;
using Shared.Features.Users;

namespace Shared.Persistence.Identity.Features.Roles;

public class RolesMapperProfile : DtoMapperProfile<FindRoleDto, CreateUserDto, UpdateRoleDto, RoleDocument>
{
}