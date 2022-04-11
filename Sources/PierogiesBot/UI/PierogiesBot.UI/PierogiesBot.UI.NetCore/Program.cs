using System;
using Autofac;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Shared.UI.Authentication;
using Shared.UI.NetCore;
using Splat;

namespace PierogiesBot.UI.NetCore
{
    public static class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            IconProvider.Register<MaterialDesignIconProvider>();
            App.ConfigurePlatformServices = services =>
            {
                services.AddSingleton<IAuthenticationStore, LiteDbAuthenticationStore>();
            };
            BuildAvaloniaApp()
               .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace()
                         .UseReactiveUI();
    }
}