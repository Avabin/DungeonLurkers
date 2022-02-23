using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using ReactiveUI;
using Shared.UI.Authentication;
using Shared.UI.IoC;
using Shared.UI.Web;
using Splat;

namespace PierogiesBot.UI.Web;

public static class Program
{
    public static async Task Main(string[] args)
    {
        IconProvider.Register<MaterialDesignIconProvider>();
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
               .AddInfrastructure()
               .AddSingleton<IAuthenticationStore, LocalStorageAuthenticationStore>();
        
        return builder;
    }

    private static void ConfigureContainer(ContainerBuilder builder)
    {
        Locator.CurrentMutable.RegisterLazySingleton(() => new AutofacViewLocator(), typeof(IViewLocator));
        builder.AddPierogiesBotUiInfrastructure();
    }
}