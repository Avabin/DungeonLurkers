using System.Reactive;
using Microsoft.Extensions.Logging;
using PierogiesBot.UI.ViewModels.Features.BotCrontabRules;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.UI;
using Shared.UI.HostScreen;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.ViewModels.HostScreen;
using Shared.UI.ViewModels.ProfileView;

namespace PierogiesBot.UI.ViewModels.Features.NavigationView;

public class NavigationViewModel : ViewModelBase
{
    public ProfileViewModel      ProfileViewModel      { get; }
    public CrontabRulesViewModel CrontabRulesViewModel { get; }
    public override   string                UrlPathSegment        => "nav";

    public NavigationViewModel(IHostScreenViewModel hostScreenViewModel, ProfileViewModel profileViewModel, CrontabRulesViewModel crontabRulesViewModel) : base(hostScreenViewModel)
    {
        ProfileViewModel = profileViewModel;
        CrontabRulesViewModel = crontabRulesViewModel;
    }
}