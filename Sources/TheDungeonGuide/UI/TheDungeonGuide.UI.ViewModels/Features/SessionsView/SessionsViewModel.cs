using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.UI;
using Shared.UI.HostScreen;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.UserStore;
using TheDungeonGuide.UI.Shared.Features.Sessions;

namespace TheDungeonGuide.UI.ViewModels.Features.SessionsView;

public class SessionsViewModel : ViewModelBase, IDisposable
{
    private readonly SourceList<Session>                   _sessions;
    private readonly CompositeDisposable                   _compositeDisposable;
    public           ObservableCollectionExtended<Session> Sessions { get; }

    [Reactive] public string                                     CurrentUserId { get; set; } = "";
    public SessionsViewModel(ISessionsService     sessionsService, IUserStore userStore,
                             IHostScreenViewModel hostScreenViewModel) :
        base(hostScreenViewModel)
    {
        _compositeDisposable = new CompositeDisposable();
        _sessions            = new SourceList<Session>();
        Sessions             = new ObservableCollectionExtended<Session>();

        _sessions.Connect()
                 .Bind(Sessions)
                 .Subscribe()
                 .DisposeWith(_compositeDisposable);

        var userIdObservable = userStore.UserInfoObservable
                                        .Select(x => x.Id);

        userIdObservable.BindTo(this, vm => vm.CurrentUserId)
                        .DisposeWith(_compositeDisposable);

        userIdObservable
           .Select(x => Observable.Defer(() => sessionsService.GetSessionsByUserId(x).ToObservable()))
           .Concat()
           .Select(x => x.Select(s => Session.Of(s, CurrentUserId)))
           .ObserveOn(RxApp.MainThreadScheduler)
           .Do(x => _sessions.EditDiff(x))
           .Subscribe()
           .DisposeWith(_compositeDisposable);
    }

    public override string UrlPathSegment => "sessions";

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _compositeDisposable.Dispose();
            _sessions.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}