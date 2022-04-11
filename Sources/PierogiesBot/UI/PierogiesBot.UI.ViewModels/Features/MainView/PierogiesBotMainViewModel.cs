﻿using System.Reactive;
using Microsoft.Extensions.Logging;
using PierogiesBot.UI.ViewModels.Features.BotCrontabRules;
using PierogiesBot.UI.ViewModels.Features.NavigationView;
using ReactiveUI;
using Shared.UI.Authentication;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.ViewModels.MainView;

namespace PierogiesBot.UI.ViewModels.Features.MainView;

public class PierogiesBotMainViewModel : MainViewModel
{
    public override    ReactiveCommand<Unit, IRoutableViewModel> NavigateToAfterLoginCommand { get; }

    public PierogiesBotMainViewModel(ILogger<PierogiesBotMainViewModel> logger, IAuthenticationStore authenticationStore, IRoutableViewModelFactory routableViewModelFactory, IMessageBus messageBus) : base(logger, authenticationStore, routableViewModelFactory, messageBus)
    {
        NavigateToAfterLoginCommand = ReactiveCommand.CreateFromObservable(Navigate<CrontabRulesViewModel>);
    }
}