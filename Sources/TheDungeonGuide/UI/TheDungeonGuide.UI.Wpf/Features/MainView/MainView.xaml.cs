using System;
using System.Windows;
using ReactiveUI;
using TheDungeonGuide.UI.ViewModels.Features.MainView;

namespace TheDungeonGuide.UI.Wpf.Features.MainView;

public partial class MainView
{
    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;

        this.WhenActivated(ConfigureSubscriptions);
    }

    private void ConfigureSubscriptions(Action<IDisposable> d)
    {
        d(this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router));
        d(this.OneWayBind(ViewModel, vm => vm.Url,    v => v.RouteTextBox.Text));
        d(this.BindCommand(ViewModel, vm => vm.NavigateBackCommand,   v => v.BackButton));
        d(this.BindCommand(ViewModel, vm => vm.GoToCharactersCommand, v => v.CharactersButton));
        d(this.BindCommand(ViewModel, vm => vm.GoToSessionsCommand,   v => v.SessionsButton));

        d(ViewModel!.GoToLoginCommand.Execute().Subscribe());
    }
}