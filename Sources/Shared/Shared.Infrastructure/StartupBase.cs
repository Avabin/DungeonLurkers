using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.Infrastructure;

public abstract class StartupBase
{
    public IConfiguration Configuration { get; }
    /// <summary>
    ///     For integration testing set this to WebApplicationFactory Client
    /// </summary>
    public static HttpClient? IdentityHttpClient { get; set; }
    public IHostEnvironment Environment { get; }
    
    protected StartupBase(IConfiguration configuration, IHostEnvironment environment)
    {
        Configuration = configuration;
        Environment   = environment;
    }
}