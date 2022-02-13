using TheDungeonGuide.UI.Shared.Features.HostScreen;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;

namespace TheDungeonGuide.UI.ViewModels.Features.CharactersView;

public class CharactersViewModel : RoutableViewModelBase
{
    public CharactersViewModel(IHostScreenViewModel hostScreenViewModel) : base(hostScreenViewModel)
    {
    }

    public override string UrlPathSegment => "characters";
}