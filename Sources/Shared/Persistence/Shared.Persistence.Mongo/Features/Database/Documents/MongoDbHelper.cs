using Shared.Persistence.Core.Features.Documents;

namespace Shared.Persistence.Mongo.Features.Database.Documents;

public static class MongoDbHelper
{
    public static string   GetCollectionName<T>() where T : IDocument<string> => typeof(T).Name.Replace("Document", "s");
    public static string DatabaseName = "Database";
}