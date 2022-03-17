using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotCrontabRules;
using PierogiesBot.UI.ViewModels.Features.BotCrontabRules;
using PierogiesBot.UI.ViewModels.Features.MainView;
using PierogiesBot.UI.ViewModels.Features.NavigationView;
using Shared.UI.ViewModels.LoginView;
using Shared.UI.ViewModels.ProfileView;
using Splat;

namespace PierogiesBot.UI.Views;

public static class DesignData
{
    public static CrontabRule               CrontabRule      => new(new BotCrontabRuleDto
    {
        Id = "123123123",
        Crontab = "* * * * *",
        IsEmoji = false,
        ReplyEmojis = {},
        ReplyMessages = {"hello"},
        ResponseMode = ResponseMode.First
    });
    public static NavigationViewModel       NavigationViewModel     => Vm<NavigationViewModel>();
    public static AddCrontabRuleViewModel   AddCrontabRuleViewModel => Vm<AddCrontabRuleViewModel>();
    public static LoginViewModel            LoginViewModel          => Vm<LoginViewModel>();
    public static PierogiesBotMainViewModel MainViewModel           => Vm<PierogiesBotMainViewModel>();
    public static ProfileViewModel          ProfileViewModel        => Vm<ProfileViewModel>();

    public static CrontabRulesViewModel CrontabRulesViewModel => Vm<CrontabRulesViewModel>();


    private static T Vm<T>() => Locator.GetLocator().GetService<T>()!;
}