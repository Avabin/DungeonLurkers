using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#pragma warning disable 8600

namespace TheDungeonGuide.UI.Shared.Features.HostBuilder;

/// <summary>
///     Extensions to emulate a typical "Startup.cs" pattern for <see cref="IHostBuilder" />
/// </summary>
public static class HostBuilderExtensions
{
    private const string ConfigureServicesMethodName = "ConfigureServices";

    /// <summary>
    ///     Specify the startup type to be used by the host.
    /// </summary>
    /// <typeparam name="TStartup">
    ///     The type containing an optional constructor with
    ///     an <see cref="IConfiguration" /> parameter. The implementation should contain a public
    ///     method named ConfigureServices with <see cref="IServiceCollection" /> parameter.
    /// </typeparam>
    /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to initialize with TStartup.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder" /> for chaining.</returns>
    public static IHostBuilder UseStartup<TStartup>(
        this IHostBuilder hostBuilder) where TStartup : class
    {
        // Invoke the ConfigureServices method on IHostBuilder...
        hostBuilder.ConfigureServices((ctx, serviceCollection) =>
        {
            // Find a method that has this signature: ConfigureServices(IServiceCollection)
            var cfgServicesMethod = typeof(TStartup).GetMethod(
                ConfigureServicesMethodName, new[] {typeof(IServiceCollection)});

            // Check if TStartup has a ctor that takes a IConfiguration parameter
            var hasConfigCtor = typeof(TStartup).GetConstructor(
                new[] {typeof(IConfiguration)}) is not null;
                
            var hasEnvCtor = typeof(TStartup).GetConstructor(
                new[] {typeof(IHostEnvironment)}) is not null;

            var hasBothCtor =
                typeof(TStartup)
                        .GetConstructor(new[] { typeof(IConfiguration), typeof(IHostEnvironment) }) 
                    is not null;
                
            var hasBothCtorInverse =
                typeof(TStartup)
                        .GetConstructor(new[] { typeof(IHostEnvironment), typeof(IConfiguration) }) 
                    is not null;

            object[] args = {ctx.Configuration, ctx.HostingEnvironment };

            // create a TStartup instance based on ctor
            TStartup startup = null;
            if (hasConfigCtor)
                startup = (TStartup) Activator.CreateInstance(typeof(TStartup), args[0]);
            else if (hasBothCtor)
                startup = (TStartup) Activator.CreateInstance(typeof(TStartup), args);
            else if (hasEnvCtor)
                startup = (TStartup) Activator.CreateInstance(typeof(TStartup), args[1]);
            else if (hasBothCtorInverse)
                startup = (TStartup) Activator.CreateInstance(typeof(TStartup), args[1], args[0]);
            else if ((hasConfigCtor, hasEnvCtor, hasBothCtor) is (false, false, false)) startup = (TStartup)Activator.CreateInstance(typeof(TStartup));

            // finally, call the ConfigureServices implemented by the TStartup object
            cfgServicesMethod?.Invoke(startup, new object[] {serviceCollection});
        });

        // chain the response
        return hostBuilder;
    }
}