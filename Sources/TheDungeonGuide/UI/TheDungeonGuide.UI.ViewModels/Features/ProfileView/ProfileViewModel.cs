using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.Features.Authentication;
using TheDungeonGuide.UI.Shared.Features.HostScreen;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;
using TheDungeonGuide.UI.Shared.Features.UserStore;

namespace TheDungeonGuide.UI.ViewModels.Features.ProfileView;

public class ProfileViewModel : RoutableViewModelBase, IDisposable
{
    private readonly  IDisposable _sub;
    [Reactive] public string      Username { get; set; }
    public ProfileViewModel(IUserStore userStore, IHostScreenViewModel hostScreenViewModel) : base(hostScreenViewModel)
    {
        _sub = userStore.UserInfoObservable
                        .Select(x => x.UserName)
                        .BindTo(this, vm => vm.Username);
    }

    public override string UrlPathSegment => "me";

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}