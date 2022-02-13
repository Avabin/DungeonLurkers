using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using TheDungeonGuide.UI.ViewModels.Features.MainView;
using TheDungeonGuide.UI.Wpf.Features.MainView;

namespace TheDungeonGuide.UI.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly HostedWpfApplication<Startup> HostedWpfApplication = new();
        public static           IServiceProvider              Container                         => HostedWpfApplication<Startup>.Container;
        public static           T                             GetService<T>() where T : notnull => Container.GetRequiredService<T>();

        public static T GetView<T>() where T : IViewFor => HostedWpfApplication.GetView<T>();
        private async void App_OnStartup(object sender, StartupEventArgs e)
        {
            await HostedWpfApplication.InitializeAsync();

            var mainWindow = HostedWpfApplication.GetView<MainView>();
            MainWindow = mainWindow;

            MainWindow.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e) => HostedWpfApplication.Stop();
    }
}