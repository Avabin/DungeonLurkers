using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Shared.Persistence.Mongo.Features.Database.Documents;
using Shared.Persistence.Mongo.Features.Database.Repository;

namespace Shared.Persistence.Mongo.Features;

public static class ContainerBuilderExtensions
{
    public static void AddPersistenceMongo(this ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(MongoRepositoryWithMessageBroker<>)).SingleInstance().AsImplementedInterfaces();
        builder.RegisterGeneric(typeof(MongoDocumentOperationFacade<,>)).AsImplementedInterfaces();
        builder.Register(ctx =>
        {
            var config           = ctx.Resolve<IConfiguration>();
            var connectionString = config.GetConnectionString("MongoDb");
            return new MongoClient(connectionString);
        }).SingleInstance().AsImplementedInterfaces();
    }
}