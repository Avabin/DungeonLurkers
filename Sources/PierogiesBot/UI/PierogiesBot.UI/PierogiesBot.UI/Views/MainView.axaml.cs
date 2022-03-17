using System;
using System.Reactive.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PierogiesBot.UI.ViewModels.Features.MainView;
using ReactiveUI;
using Shared.UI.IoC;
using Shared.UI.ViewModels.MainView;

namespace PierogiesBot.UI.Views;

public partial class MainView : ReactiveUserControl<PierogiesBotMainViewModel>
{
    public MainView() : this(ServiceLocator.GetRequiredService<PierogiesBotMainViewModel>()) {}
    public MainView(PierogiesBotMainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        this.WhenActivated(d =>
        {
            
        });
    }
        
    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}