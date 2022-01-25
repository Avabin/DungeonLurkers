using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Shared.Persistence.Mongo.Features.Database;
using Shared.Persistence.Mongo.Features.Database.Documents;
using Shared.Persistence.Mongo.Features.Database.Repository;

namespace Shared.Persistence.Mongo.Features;

public static class ContainerBuilderExtensions
{
    public static void AddPersistenceMongo(this ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(MongoRepository<>)).AsImplementedInterfaces();
        builder.RegisterGeneric(typeof(MongoDocumentOperationFacade<,>)).AsImplementedInterfaces();
        builder.RegisterType<Mongo2GoService>().SingleInstance();
        builder.Register(ctx =>
        {
            var env              = ctx.Resolve<IHostEnvironment>();
            var config           = ctx.Resolve<IConfiguration>();
            var connectionString = config.GetConnectionString("MongoDb");

            if (!env.IsDevelopment() || config["UseMongo2Go"] != bool.TrueString)
                return new MongoClient(connectionString);

            var mongo      = ctx.Resolve<Mongo2GoService>();
            var connString = mongo.StartMongo();

            return new MongoClient(connString);
        }).SingleInstance().AsImplementedInterfaces();
    }
}