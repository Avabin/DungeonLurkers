using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using DynamicData;
using DynamicData.Binding;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotCrontabRules;
using PierogiesBot.UI.Shared.Features.BotCrontabRules;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using Shared.UI;
using Shared.UI.HostScreen;

namespace PierogiesBot.UI.ViewModels.Features.BotCrontabRules;

public class AddCrontabRuleViewModel : ViewModelBase
{
    private readonly ICrontabRulesService _service;

    [Reactive] public string       Crontab      { get; set; } = "";
    [Reactive] public ResponseMode ResponseMode { get; set; } = ResponseMode.First;
    [Reactive] public bool         IsEmoji      { get; set; } = false;

    private readonly SourceList<string>            _responses;
    public           IObservableCollection<string> Responses { get; }

    private readonly SourceList<string>            _emotes;
    public           IObservableCollection<string> Emotes { get; }

    public ReactiveCommand<string, Unit> AddResponseCommand    { get; }
    public ReactiveCommand<string, Unit> RemoveResponseCommand { get; }
    public ReactiveCommand<string, Unit> AddEmoteCommand       { get; }
    public ReactiveCommand<string, Unit> RemoveEmoteCommand    { get; }

    public ReactiveCommand<Unit, Unit> SaveRuleCommand { get; }
    public Interaction<BotCrontabRuleDto, Unit>                 RuleSavedInteraction{ get; }


    public AddCrontabRuleViewModel(ICrontabRulesService service, IHostScreenViewModel hostScreenViewModel) :
        base(hostScreenViewModel)
    {
        _service = service;

        _responses = new SourceList<string>();
        Responses  = new ObservableCollectionExtended<string>();
        _emotes    = new SourceList<string>();
        Emotes     = new ObservableCollectionExtended<string>();

        AddResponseCommand    = ReactiveCommand.Create<string>(AddResponse);
        RemoveResponseCommand = ReactiveCommand.Create<string>(RemoveResponse);
        AddEmoteCommand       = ReactiveCommand.Create<string>(AddEmote);
        RemoveEmoteCommand    = ReactiveCommand.Create<string>(RemoveEmote);

        SaveRuleCommand = ReactiveCommand.CreateFromTask(SaveRuleAsync);

        RuleSavedInteraction = new Interaction<BotCrontabRuleDto, Unit>();
    }

    private async Task SaveRuleAsync()
    {
        var request = new CreateBotCrontabRuleDto
        {
            Crontab       = Crontab,
            IsEmoji       = IsEmoji,
            ResponseMode  = ResponseMode,
            ReplyMessages = Responses,
            ReplyEmojis   = Emotes
        };

        var created = await _service.CreateRuleAsync(request);

        await RuleSavedInteraction.Handle(created);
    }

    private void RemoveEmote(string emote)
    {
        if (Emotes.Contains(emote)) _emotes.Remove(emote);
    }

    private void RemoveResponse(string response)
    {
        if (Responses.Contains(response)) _responses.Remove(response);
    }

    private void AddEmote(string emote)
    {
        if (!Emotes.Contains(emote))
            _emotes.Add(emote);
    }

    private void AddResponse(string response)
    {
        if (!Responses.Contains(response))
            _responses.Add(response);
    }

    public override string UrlPathSegment => "addCrontabRule";

    public static IEnumerable<ResponseMode> ResponseModes => Enum.GetValues(typeof(ResponseMode)).Cast<ResponseMode>();
    public        IEnumerable<ValidationResult>             Validate(ValidationContext validationContext)
    {
        throw new NotImplementedException();
    }
}