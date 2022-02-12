using Autofac;
using Shared.Persistence.Core.Features;
using Shared.Persistence.Mongo.Features;
using TheDungeonGuide.Persistence.Characters;

namespace TheDungeonGuide.Characters.Infrastructure;

public static class ContainerBuilderExtensions
{
    public static void AddCharacters(this ContainerBuilder builder)
    {
        builder.AddPersistenceCore();
        builder.AddPersistenceMongo();
        builder.AddCharactersMongoServices();
    }
}