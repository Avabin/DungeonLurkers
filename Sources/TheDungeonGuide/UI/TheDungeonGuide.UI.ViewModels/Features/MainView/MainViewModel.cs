using System.Reactive;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using TheDungeonGuide.UI.Shared.Features.HostScreen;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;
using TheDungeonGuide.UI.ViewModels.Features.CharactersView;
using TheDungeonGuide.UI.ViewModels.Features.LoginView;
using TheDungeonGuide.UI.ViewModels.Features.ProfileView;
using TheDungeonGuide.UI.ViewModels.Features.SessionsView;

namespace TheDungeonGuide.UI.ViewModels.Features.MainView;

public class MainViewModel : DefaultHostScreenViewModel, IActivatableViewModel
{
    public ReactiveCommand<Unit, IRoutableViewModel> GoToSessionsCommand   { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoToProfileCommand   { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoToLoginCommand   { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoToCharactersCommand { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoBack         { get; }

    public MainViewModel(ILogger<MainViewModel> logger, IRoutableViewModelFactory routableViewModelFactory) :
        base(logger, routableViewModelFactory)
    {
        GoToSessionsCommand   = CreateNavigateCommand<SessionsViewModel>();
        GoToProfileCommand    = CreateNavigateCommand<ProfileViewModel>();
        GoToLoginCommand      = CreateNavigateCommand<LoginViewModel>();
        GoToCharactersCommand = CreateNavigateCommand<CharactersViewModel>();
        GoBack                = ReactiveCommand.CreateFromObservable(NavigateBack)!;
    }

    public ViewModelActivator Activator { get; }
}