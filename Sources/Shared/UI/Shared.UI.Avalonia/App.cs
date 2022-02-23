using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Shared.UI.IoC;
using Shared.UI.ViewModels.MainView;

namespace Shared.UI.Avalonia;

public abstract class AppBase<TWindow, TSingleView, TDataContext> : Application 
    where TDataContext : MainViewModel
    where TWindow : Window, new()
    where TSingleView : UserControl, new()
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new TWindow
            {
                DataContext = ServiceLocator.GetRequiredService<TDataContext>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new TSingleView
            {
                DataContext = ServiceLocator.GetRequiredService<TDataContext>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}