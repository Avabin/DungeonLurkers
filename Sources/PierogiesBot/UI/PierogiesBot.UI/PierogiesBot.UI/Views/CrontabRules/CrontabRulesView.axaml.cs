using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Aura.UI.Extensions;
using Aura.UI.Services;
using Avalonia.Controls;
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

    private IObservable<Unit> HandleConfirm(InteractionContext<CrontabRule, bool> ctx)
    {
        var subject = new Subject<Unit>();
        this.GetParentTOfLogical<Window>()
            .NewContentDialog(
                              "Are you sure you want to delete this rule? (ID: " + ctx.Input.Id + ")",
                              (_, _) =>
                              {
                                  ctx.SetOutput(true);
                                  subject.OnNext(Unit.Default);
                              },
                              (_, _) =>
                              {
                                  ctx.SetOutput(false);
                                  subject.OnNext(Unit.Default);
                              },
                              "Ok",
                              "Cancel");


        return subject;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}