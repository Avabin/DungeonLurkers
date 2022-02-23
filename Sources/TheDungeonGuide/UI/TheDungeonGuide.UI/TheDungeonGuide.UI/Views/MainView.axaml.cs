using System;
using System.Reactive.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Shared.UI.IoC;
using Shared.UI.ViewModels.MainView;

namespace TheDungeonGuide.UI.Views
{
    public partial class MainView : ReactiveUserControl<MainViewModel>
    {
        public MainView() : this(ServiceLocator.GetRequiredService<MainViewModel>()) {}
        public MainView(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(ViewModel.AuthenticationStore.IsAuthenticated
                           .Skip(1)
                           .Select(isAuthenticated => 
                                       isAuthenticated 
                                           ? ViewModel.GoToProfileCommand.Execute() 
                                           : ViewModel.GoToLoginCommand.Execute())
                           .Concat()
                           .Subscribe());
            });
        }
        
        public void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}