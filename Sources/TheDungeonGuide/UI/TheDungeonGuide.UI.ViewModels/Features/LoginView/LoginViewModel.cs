using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.Features.Authentication;
using TheDungeonGuide.UI.Shared.Features.HostScreen;
using TheDungeonGuide.UI.Shared.Features.Login;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;
using TheDungeonGuide.UI.ViewModels.Features.ProfileView;

namespace TheDungeonGuide.UI.ViewModels.Features.LoginView;

public class LoginViewModel : RoutableViewModelBase
{
    private readonly ILoginService _api;

    [Reactive] public string Username { get; set; } = "username";

    public ReactiveCommand<string, Unit>             LoginCommand { get; set; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoToProfile  { get; }

    public LoginViewModel(ILoginService api, IHostScreenViewModel hostScreenViewModel) :
        base(hostScreenViewModel)
    {
        _api = api;

        LoginCommand = ReactiveCommand.CreateFromTask<string>(LoginAsync);

        GoToProfile = CreateNavigateCommand<ProfileViewModel>();
    }

    private async Task LoginAsync(string password)
    {
        await _api.LoginAsync(Username, password);

        GoToProfile.Execute().Subscribe();
    }

    public override string UrlPathSegment => "login";
}