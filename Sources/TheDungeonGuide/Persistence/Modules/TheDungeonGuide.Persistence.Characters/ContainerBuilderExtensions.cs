using Autofac;
using Shared.Persistence.Core.Features.Documents.Many;
using TheDungeonGuide.Persistence.Characters.Features.Many;
using TheDungeonGuide.Persistence.Characters.Features.Single;

namespace TheDungeonGuide.Persistence.Characters;

public static class ContainerBuilderExtensions
{
    public static void AddCharactersMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<SingleCharacterService>().AsImplementedInterfaces();
        builder.RegisterType<ManyCharactersService>().AsImplementedInterfaces();
        builder.RegisterType<CharacterFacade>().AsImplementedInterfaces();
    }
}