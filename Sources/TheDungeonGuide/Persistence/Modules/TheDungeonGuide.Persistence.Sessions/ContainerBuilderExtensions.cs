using Autofac;
using TheDungeonGuide.Persistence.Sessions.Features.Many;
using TheDungeonGuide.Persistence.Sessions.Features.Single;

namespace TheDungeonGuide.Persistence.Sessions;

public static class ContainerBuilderExtensions
{
    public static void AddSessionsMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<ManySessionsService>().AsImplementedInterfaces();
        builder.RegisterType<SingleSessionService>().AsImplementedInterfaces();
        builder.RegisterType<SessionFacade>().AsImplementedInterfaces();
    }
}