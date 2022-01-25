using Discord.Commands;
using TimeZoneConverter;

namespace PierogiesBot.Discord.Commands.Features.TimeZoneTypeConverter
{
    public class TimeZoneInfoTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input,
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
}