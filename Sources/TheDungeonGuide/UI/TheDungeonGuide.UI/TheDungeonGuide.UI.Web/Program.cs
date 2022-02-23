using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Shared.UI.Authentication;
using Shared.UI.IoC;
using Shared.UI.ViewModels;
using Shared.UI.Web;
using Splat;

namespace TheDungeonGuide.UI.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        ServiceLocator.Instance = host.Services;

        await host.RunAsync();
    }

    public static WebAssemblyHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.ConfigureContainer(new AutofacServiceProviderFactory(ConfigureContainer));

        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
               .AddBlazoredLocalStorage()
               .AddSingleton<IAuthenticationStore, LocalStorageAuthenticationStore>();
        
        builder.Services.AddInfrastructure();
        return builder;
    }

    private static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.AddTdgUiInfrastructure();
        Locator.CurrentMutable.RegisterLazySingleton(() => new AutofacViewLocator(), typeof(IViewLocator));
    }
}