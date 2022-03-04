using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using PierogiesBot.UI.ViewModels.Features.BotCrontabRules;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.UI;
using Shared.UI.HostScreen;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.UserStore;
using Shared.UI.ViewModels.HostScreen;
using Shared.UI.ViewModels.ProfileView;

namespace PierogiesBot.UI.ViewModels.Features.NavigationView;

public class NavigationViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    [Reactive] public string Title          { get; set; } = "";
    public override   string UrlPathSegment => "nav";

    public NavigationViewModel(IUserService userService, IHostScreenViewModel hostScreenViewModel) : base(hostScreenViewModel)
    {
        this.WhenActivated(d =>
        {
            d(userService.UserInfoObservable
                         .Select(x => x.UserName)
                         .Select(x => $"PierogiesBot\\{x}")
                         .BindTo(this, x => x.Title));
        });
    }

}