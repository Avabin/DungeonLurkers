using Microsoft.Extensions.Logging;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;

namespace TheDungeonGuide.UI.Shared.Features.HostScreen;

public class DefaultHostScreenViewModel : HostScreenViewModelBase
{
    public DefaultHostScreenViewModel(ILogger<DefaultHostScreenViewModel> logger, IRoutableViewModelFactory routableViewModelFactory) : base(logger, routableViewModelFactory)
    {
    }
}