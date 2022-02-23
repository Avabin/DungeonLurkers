using Microsoft.Extensions.Logging;
using ReactiveUI;
using Shared.UI.Authentication;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.ViewModels.ProfileView;

namespace Shared.UI.ViewModels.MainView;

public class DefaultMainViewModel : MainViewModel
{
    public DefaultMainViewModel(ILogger<DefaultMainViewModel> logger, IAuthenticationStore authenticationStore, IRoutableViewModelFactory routableViewModelFactory, IMessageBus messageBus) : base(logger, authenticationStore, routableViewModelFactory, messageBus)
    {
    }
}