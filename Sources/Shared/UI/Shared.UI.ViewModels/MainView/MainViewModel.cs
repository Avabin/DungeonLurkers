using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.UI.Authentication;
using Shared.UI.HostScreen;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.ViewModels.LoginView;
using Shared.UI.ViewModels.ProfileView;
using DefaultHostScreenViewModel = Shared.UI.ViewModels.HostScreen.DefaultHostScreenViewModel;

namespace Shared.UI.ViewModels.MainView;

public class MainViewModel : DefaultHostScreenViewModel, IActivatableViewModel
{
    private readonly IMessageBus _messageBus;
    [Reactive] public string LoadingText { get; set; }
    public IAuthenticationStore                       AuthenticationStore   { get; }
    public ReactiveCommand<Unit, IRoutableViewModel>  GoToProfileCommand    { get; }
    public ReactiveCommand<Unit, IRoutableViewModel>  GoToLoginCommand      { get; }
    public ReactiveCommand<Unit, IRoutableViewModel?> GoBack                => Router.NavigateBack;

    public MainViewModel(ILogger<MainViewModel> logger, IAuthenticationStore authenticationStore, IRoutableViewModelFactory routableViewModelFactory, IMessageBus messageBus) :
        base(logger, routableViewModelFactory)
    {
        _messageBus = messageBus;
        AuthenticationStore = authenticationStore;
        GoToProfileCommand  = CreateNavigateCommand<ProfileViewModel>();
        GoToLoginCommand    = CreateNavigateCommand<LoginViewModel>();

        this.WhenActivated(d =>
        {
            d(Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Select(x => (int)x % 4)
                .Select(x => $"Loading{string.Concat(Enumerable.Repeat(".", x))}")
                .BindTo(this,vm => vm.LoadingText));

            // Login when token is not saved or token is expired
            d(_messageBus.Listen<UserLoginNeeded>()
                .Select(_ => Unit.Default)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(GoToLoginCommand));

            // User is authenticated, navigate to profile
            d(authenticationStore
                .IsAuthenticated
                .Where(x => x)
                .Select(_ => Unit.Default)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(GoToProfileCommand));

            d(authenticationStore
                .Initialize()
                .ToObservable()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe());
        });
    }

    public ViewModelActivator Activator { get; } = new();
}