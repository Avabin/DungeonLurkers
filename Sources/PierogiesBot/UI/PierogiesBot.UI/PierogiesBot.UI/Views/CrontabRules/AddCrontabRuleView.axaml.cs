using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PierogiesBot.UI.ViewModels.Features.BotCrontabRules;
using ReactiveUI;
using Shared.UI.IoC;

namespace PierogiesBot.UI.Views.CrontabRules;

public partial class AddCrontabRuleView : ReactiveUserControl<AddCrontabRuleViewModel>
{
    public AddCrontabRuleView() : this(ServiceLocator.GetRequiredService<AddCrontabRuleViewModel>()) {}
    public AddCrontabRuleView(AddCrontabRuleViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(ViewModel.RuleSavedInteraction.RegisterHandler(x =>
            {
                var ruleId = x.Input.Id;
                this.ShowMessageBox("Rule with ID " + ruleId + " has been saved.", "Rule saved");
            }));
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}