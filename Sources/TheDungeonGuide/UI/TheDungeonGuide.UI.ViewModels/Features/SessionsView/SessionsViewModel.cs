using TheDungeonGuide.UI.Shared.Features.HostScreen;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;

namespace TheDungeonGuide.UI.ViewModels.Features.SessionsView;

public class SessionsViewModel : RoutableViewModelBase
{
    public SessionsViewModel(IHostScreenViewModel hostScreenViewModel) : base(hostScreenViewModel)
    {
    }

    public override string UrlPathSegment => "sessions";
}