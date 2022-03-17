using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using Shared.UI.IoC;
using Shared.UI.ViewModels.MainView;

namespace Shared.UI.Avalonia;

public abstract class AppBase<TWindow, TSingleView, TDataContext> : Application 
    where TDataContext : MainViewModel
    where TWindow : Window, new()
    where TSingleView : UserControl, new()
{
    protected static IVisual XamlRoot;
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new TWindow
            {
                DataContext = ServiceLocator.GetRequiredService<TDataContext>()
            };
            XamlRoot = desktop.MainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new TSingleView
            {
                DataContext = ServiceLocator.GetRequiredService<TDataContext>()
            };
            XamlRoot = singleViewPlatform.MainView;
        }

        base.OnFrameworkInitializationCompleted();
    }
}