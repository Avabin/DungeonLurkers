using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PierogiesBot.UI.ViewModels.Features.NavigationView;
using Shared.UI.IoC;

namespace PierogiesBot.UI.Views.NavigationView;

public partial class NavigationView : ReactiveUserControl<NavigationViewModel>
{
    public NavigationView() : this(ServiceLocator.GetRequiredService<NavigationViewModel>()) {}
    public NavigationView(NavigationViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}