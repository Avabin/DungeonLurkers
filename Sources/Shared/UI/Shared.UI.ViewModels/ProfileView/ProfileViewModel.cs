using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.UI.HostScreen;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.UserStore;

namespace Shared.UI.ViewModels.ProfileView;

public class ProfileViewModel : ViewModelBase, IActivatableViewModel
{
    public override   string             UrlPathSegment => "me";
    public            ViewModelActivator Activator      { get; }      = new();
    [Reactive] public string             Id             { get; set; } = "";
    [Reactive] public string             Username       { get; set; } = "";
    [Reactive] public string             Email          { get; set; } = "";
    public ProfileViewModel(IUserService userService, IHostScreenViewModel hostScreenViewModel) : base(hostScreenViewModel)
    {
        this.WhenActivated(d =>
        {
            d(userService.UserInfoObservable
                         .Select(x => x.UserName)
                         .BindTo(this, vm => vm.Username));

            d(userService.UserInfoObservable.Select(x => x.Email)
                         .BindTo(this, vm => vm.Email));

            d(userService.UserInfoObservable.Select(x => x.Id)
                         .BindTo(this, vm => vm.Id));

            d(userService.FetchProfile().Subscribe());
        });
    }
    
    
}