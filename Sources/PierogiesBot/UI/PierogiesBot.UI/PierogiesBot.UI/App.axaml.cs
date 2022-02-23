using System.Linq;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PierogiesBot.UI.Views;
using ReactiveUI;
using Shared.UI.IoC;
using Shared.UI.ViewModels.MainView;

namespace PierogiesBot.UI
{
    public partial class App : Application
    {
        public static void AddViews(ContainerBuilder builder)
        {
            var assembly = typeof(MainView).Assembly;
        
            // Load views
            builder.RegisterAssemblyTypes(assembly)
                   .Where(t => t.GetInterfaces()
                                .Any(
                                     i => i.IsGenericType
                                       && i.GetGenericTypeDefinition() == typeof(IViewFor<>)))
                   .AsSelf()
                   .AsImplementedInterfaces();
        }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = ServiceLocator.GetRequiredService<MainViewModel>()
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = ServiceLocator.GetRequiredService<MainViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}