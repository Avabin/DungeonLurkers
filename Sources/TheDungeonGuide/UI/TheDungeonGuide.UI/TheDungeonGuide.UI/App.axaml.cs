using Avalonia.Markup.Xaml;
using Shared.UI.Avalonia;
using Shared.UI.ViewModels.MainView;
using TheDungeonGuide.UI.Views;

namespace TheDungeonGuide.UI;

public partial class App : AppBase<MainWindow, MainView, DefaultMainViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}