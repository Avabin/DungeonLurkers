using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotCrontabRules;
using PierogiesBot.UI.Shared;
using PierogiesBot.UI.Shared.Features.BotCrontabRules;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.UI;
using Shared.UI.HostScreen;

namespace PierogiesBot.UI.ViewModels.Features.BotCrontabRules;

public class CrontabRulesViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly ICrontabRulesService           _service;
    private readonly ILogger<CrontabRulesViewModel> _logger;
    public override  string                         UrlPathSegment => "crontabRules";

    public ViewModelActivator Activator { get; } = new();

    private SourceList<CrontabRule>            _rules;
    public  IObservableCollection<CrontabRule> Rules         { get; }
    public  List<ResponseMode>                 ResponseModes { get; }

    public ReactiveCommand<string, Unit>                         AddResponseToRuleCommand      { get; }
    public ReactiveCommand<string, Unit>                         RemoveResponseFromRuleCommand { get; }
    public ReactiveCommand<Unit, IEnumerable<BotCrontabRuleDto>> ReloadRulesCommand            { get; }

    [Reactive] public CrontabRule? SelectedRule { get; set; }

    public ReactiveCommand<string, Unit>      AddEmoteToRuleCommand        { get; }
    public ReactiveCommand<string, Unit>      RemoveEmoteFromRuleCommand   { get; }
    public ReactiveCommand<CrontabRule, Unit> DeleteRuleCommand            { get; }
    public Interaction<CrontabRule, bool>     ConfirmDeleteRuleInteraction { get; }

    public CrontabRulesViewModel(ICrontabRulesService           service, IHostScreenViewModel hostScreenViewModel,
                                 ILogger<CrontabRulesViewModel> logger) : base(hostScreenViewModel)
    {
        _service      = service;
        _logger       = logger;
        _rules        = new SourceList<CrontabRule>();
        Rules         = new ObservableCollectionExtended<CrontabRule>();
        ResponseModes = Enum.GetValues(typeof(ResponseMode)).Cast<ResponseMode>().ToList();

        var canModifyRule = this.WhenAnyValue(x => x.SelectedRule).Select(x => x is not null);
        AddResponseToRuleCommand = ReactiveCommand.CreateFromTask<string>(AddResponseToRuleAsync, canModifyRule);
        RemoveResponseFromRuleCommand =
            ReactiveCommand.CreateFromTask<string>(RemoveResponseFromRuleAsync, canModifyRule);
        AddEmoteToRuleCommand = ReactiveCommand.CreateFromTask<string>(AddEmoteToRuleAsync, canModifyRule);
        RemoveEmoteFromRuleCommand =
            ReactiveCommand.CreateFromTask<string>(RemoveEmoteFromRuleAsync, canModifyRule);
        ReloadRulesCommand           = ReactiveCommand.CreateFromTask(GetRulesAsync);
        DeleteRuleCommand            = ReactiveCommand.CreateFromTask<CrontabRule>(DeleteRuleAsync, canModifyRule);
        ConfirmDeleteRuleInteraction = new Interaction<CrontabRule, bool>();

        this.WhenActivated(d =>
        {
            Rules.Clear();
            d(_rules.Connect().Bind(Rules).Subscribe());
            d(ReloadRulesCommand
             .ObserveOn(RxApp.MainThreadScheduler)
             .Do(x => _rules.Edit(y =>
              {
                  var crontabRules = x.ToList();

                  foreach (var crontabRule in crontabRules)
                  {
                      var newRule      = CrontabRule.From(crontabRule);
                      var existingRule = Rules.SingleOrDefault(z => z.Id == newRule.Id);
                      if (existingRule is null) y.Add(newRule);
                      else
                      {
                          if (!existingRule.Equals(newRule))
                            y.Replace(existingRule, newRule);
                      }
                  }
              }))
             .Subscribe());

            d(ReloadRulesCommand.Execute().Subscribe());
        });
    }

    private async Task DeleteRuleAsync(CrontabRule rule)
    {
        var ruleId = rule.Id;
        // Wait for user confirmation
        await ConfirmDeleteRuleInteraction.Handle(rule);
        // await _service.DeleteRuleAsync(ruleId);
        // _rules.Edit(x =>
        // {
            // var crontabRule = x.SingleOrDefault(y => y.Id == ruleId);
            // if (crontabRule is not null) x.Remove(crontabRule);
        // });
    }

    private async Task AddEmoteToRuleAsync(string emote)
    {
        if (Rules.Any(x => x.ReplyEmojis.Contains(emote)))
            return;
        _logger.LogDebug("Adding emote {Emote} to rule {SelectedRuleId}", emote, SelectedRule?.Id);
        if (SelectedRule is null)
            return;
        await _service.AddEmoteToRuleAsync(SelectedRule.Id, emote);
        _rules.Edit(list => list.Single(x => x.Id == SelectedRule.Id).ReplyEmojis.Add(emote));
    }

    private async Task RemoveEmoteFromRuleAsync(string emote)
    {
        if (!Rules.Any(x => x.ReplyEmojis.Contains(emote)))
            return;
        _logger.LogDebug("Removing emote {Emote} from rule {SelectedRuleId}", emote, SelectedRule?.Id);
        if (SelectedRule is null)
            return;
        await _service.RemoveEmoteFromRuleAsync(SelectedRule.Id, emote);
        _rules.Edit(list => list.Single(x => x.Id == SelectedRule.Id).ReplyEmojis.Remove(emote));
    }

    private async Task<IEnumerable<BotCrontabRuleDto>> GetRulesAsync() =>
        await _service.GetAllAsync();

    private async Task RemoveResponseFromRuleAsync(string response)
    {
        if (!Rules.Any(x => x.ReplyMessages.Contains(response)))
            return;
        _logger.LogDebug("Removing response {Response} from rule {SelectedRuleId}", response, SelectedRule?.Id);
        if (SelectedRule is null)
            return;
        await _service.RemoveResponseFromRuleAsync(SelectedRule.Id, response);
        _rules.Edit(list => list.Single(x => x.Id == SelectedRule.Id).ReplyMessages.Remove(response));
    }

    private async Task AddResponseToRuleAsync(string response)
    {
        if (Rules.Any(x => x.ReplyMessages.Contains(response)))
            return;
        _logger.LogDebug("Adding response {Response} to rule {SelectedRuleId}", response, SelectedRule?.Id);
        if (SelectedRule is null)
            return;
        await _service.AddResponseToRuleAsync(SelectedRule.Id, response);
        _rules.Edit(list => list.Single(x => x.Id == SelectedRule.Id).ReplyMessages.Add(response));
    }
}