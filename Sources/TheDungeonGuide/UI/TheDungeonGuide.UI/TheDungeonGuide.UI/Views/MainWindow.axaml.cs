using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Shared.UI.ViewModels.MainView;

namespace TheDungeonGuide.UI.Views;

public partial class MainWindow : ReactiveWindow<DefaultMainViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}