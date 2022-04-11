using Shared.UI;
using Shared.UI.HostScreen;
using Shared.UI.Navigation.RoutableViewModel;

namespace TheDungeonGuide.UI.ViewModels.Features.CharactersView;

public class CharactersViewModel : ViewModelBase
{
    public CharactersViewModel(IHostScreenViewModel hostScreenViewModel) : base(hostScreenViewModel)
    {
    }

    public override string UrlPathSegment => "characters";
}