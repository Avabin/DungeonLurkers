using Discord;
using Discord.Commands;
using Discord.Interactions;
using TimeZoneConverter;

namespace PierogiesBot.Discord.Core.Features.TimeZoneTypeConverter;

public class TimeZoneInfoTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext  context, string input,
                                                     IServiceProvider services)
    {
        try
        {
            var tzInfo = TZConvert.GetTimeZoneInfo(input);
            return Task.FromResult(TypeReaderResult.FromSuccess(tzInfo));
        }
        catch (Exception e)
        {
            return Task.FromResult(TypeReaderResult.FromError(e));
        }
    }
}

public class TimeZoneInfoConverter : TypeConverter<TimeZoneInfo>
{
    public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;

    public override Task<TypeConverterResult>    ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
    {
        if(option.Value is not string timeZoneId) return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.BadArgs, "timezone ID must be string"));
        var tzInfo = TZConvert.GetTimeZoneInfo(timeZoneId);
        return Task.FromResult(TypeConverterResult.FromSuccess(tzInfo));
    }
}