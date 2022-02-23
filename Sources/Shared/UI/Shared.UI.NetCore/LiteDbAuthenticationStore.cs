using LiteDB;
using Microsoft.Extensions.FileProviders;
using Shared.Features.Authentication;
using Shared.UI.Authentication;
using Shared.UI.Users;
using Shared.UI.UserStore;

namespace Shared.UI.NetCore;

public class LiteDbAuthenticationStore : AuthenticationStoreBase, IDisposable
{
    private readonly ILiteDatabase _db;
    private ILiteCollection<AuthenticationState> Collection => _db.GetCollection<AuthenticationState>("authentication");

    public LiteDbAuthenticationStore(IFileProvider fileProvider, IAuthenticatedApi api) : base(api)
    {
        var dbFile = fileProvider.GetFileInfo("litedb.db");
        _db = new LiteDatabase(dbFile.PhysicalPath);
    }

    protected override Task SaveToken(AuthenticationState state)
    {
        Collection.Upsert(state);
        return Task.CompletedTask;
    }

    protected override Task<AuthenticationState?> LoadToken()
    {
        var state = Collection.Find(x => true).SingleOrDefault();

        return Task.FromResult(state);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _db.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}