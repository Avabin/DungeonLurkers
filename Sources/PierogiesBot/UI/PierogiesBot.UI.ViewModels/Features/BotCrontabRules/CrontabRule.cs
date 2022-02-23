using DynamicData.Binding;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotCrontabRules;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PierogiesBot.UI.ViewModels.Features.BotCrontabRules;

public class CrontabRule : ReactiveObject, IEquatable<CrontabRule>
{
    public bool Equals(CrontabRule? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return IsEmoji == other.IsEmoji && Crontab == other.Crontab && ReplyMessages.Equals(other.ReplyMessages) && ReplyEmojis.Equals(other.ReplyEmojis) && ResponseMode == other.ResponseMode && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CrontabRule)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsEmoji, Crontab, ReplyMessages, ReplyEmojis, (int)ResponseMode, Id);
    }

    public static bool operator ==(CrontabRule? left, CrontabRule? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CrontabRule? left, CrontabRule? right)
    {
        return !Equals(left, right);
    }

    [Reactive] public bool                          IsEmoji       { get; set; }
    [Reactive] public string                        Crontab       { get; set; }
    public            IObservableCollection<string> ReplyMessages { get; }
    public            IObservableCollection<string> ReplyEmojis   { get; }
    [Reactive] public ResponseMode                  ResponseMode  { get; set; }
    [Reactive] public string                        Id            { get; set; }
    public CrontabRule(BotCrontabRuleDto dto)
    {
        IsEmoji = dto.IsEmoji;
        Crontab = dto.Crontab;
        ResponseMode = dto.ResponseMode;
        Id = dto.Id;
        ReplyMessages = new ObservableCollectionExtended<string>(dto.ReplyMessages);
        ReplyEmojis = new ObservableCollectionExtended<string>(dto.ReplyEmojis);
    }

    public static CrontabRule From(BotCrontabRuleDto dto) => new(dto);
}