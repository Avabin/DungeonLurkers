using Microsoft.Extensions.Logging;
using Shared.UI.Navigation.RoutableViewModel;

namespace Shared.UI.ViewModels.HostScreen;

public class DefaultHostScreenViewModel : HostScreenViewModelBase
{
    public DefaultHostScreenViewModel(ILogger<DefaultHostScreenViewModel> logger, IRoutableViewModelFactory routableViewModelFactory) : base(logger, routableViewModelFactory)
    {
    }
}