using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.UI.HostScreen;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.UserStore;

namespace Shared.UI.ViewModels.ProfileView;

public class ProfileViewModel : ViewModelBase, IDisposable
{
    private readonly  CompositeDisposable _subs;
    [Reactive] public string              Id       { get; set; } = "";
    [Reactive] public string              Username { get; set; } = "";
    [Reactive] public string              Email    { get; set; } = "";
    public ProfileViewModel(IUserStore userStore, IHostScreenViewModel hostScreenViewModel) : base(hostScreenViewModel)
    {
        _subs = new CompositeDisposable();
        
        userStore.UserInfoObservable
                 .Select(x => x.UserName)
                 .BindTo(this, vm => vm.Username)
                 .DisposeWith(_subs);
        
        userStore.UserInfoObservable.Select(x => x.Email)
                 .BindTo(this, vm => vm.Email)
                 .DisposeWith(_subs);

        userStore.UserInfoObservable.Select(x => x.Id)
                 .BindTo(this, vm => vm.Id)
                 .DisposeWith(_subs);
    }
    
    public override string UrlPathSegment => "me";

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subs.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}