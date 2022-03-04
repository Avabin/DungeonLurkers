using System;
using System.Reactive;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PierogiesBot.UI.ViewModels.Features.BotCrontabRules;
using ReactiveUI;
using Shared.UI.IoC;

namespace PierogiesBot.UI.Views.CrontabRules;

public partial class CrontabRulesView : ReactiveUserControl<CrontabRulesViewModel>
{
    public CrontabRulesView() : this(ServiceLocator.GetRequiredService<CrontabRulesViewModel>())
    {
    }

    public CrontabRulesView(CrontabRulesViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        this.WhenActivated(d => { d(ViewModel.ConfirmDeleteRuleInteraction.RegisterHandler(HandleConfirm)); });
    }

    private IObservable<Unit> HandleConfirm(InteractionContext<CrontabRule, bool> ctx) =>
        this.ShowDialogAndWait("Are you sure you want to delete this rule? ID: " + ctx.Input.Id, 
                               () => ctx.SetOutput(true),
                               () => ctx.SetOutput(false),
                               "Yes", "No");

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}