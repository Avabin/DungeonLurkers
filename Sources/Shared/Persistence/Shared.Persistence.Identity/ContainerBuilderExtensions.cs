using Autofac;
using Shared.Persistence.Identity.Features.Roles;
using Shared.Persistence.Identity.Features.Roles.Many;
using Shared.Persistence.Identity.Features.Roles.Single;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Identity.Features.Users.Many;
using Shared.Persistence.Identity.Features.Users.Single;

namespace Shared.Persistence.Identity;

public static class ContainerBuilderExtensions
{
    public static void AddIdentityMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<SingleRoleService>().AsImplementedInterfaces();
        builder.RegisterType<ManyRolesService>().AsImplementedInterfaces();
        builder.RegisterType<RoleFacade>().AsImplementedInterfaces();
        builder.RegisterType<SingleUserService>().AsImplementedInterfaces();
        builder.RegisterType<ManyUsersService>().AsImplementedInterfaces();
        builder.RegisterType<UserFacade>().AsImplementedInterfaces();
    }
}