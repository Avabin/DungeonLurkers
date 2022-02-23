using System.Linq;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PierogiesBot.UI.ViewModels.Features.MainView;
using PierogiesBot.UI.Views;
using ReactiveUI;
using Shared.UI.Avalonia;
using Shared.UI.IoC;
using Shared.UI.ViewModels.MainView;

namespace PierogiesBot.UI;

public partial class App : AppBase<MainWindow, MainView, PierogiesBotMainViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}