using System.Runtime.Serialization;

namespace TheDungeonGuide.Persistence.Sessions.Features.Exceptions;

public class MemberNotFoundException : Exception
{
    public MemberNotFoundException(string memberId) : base($"Member with id {memberId} not found")
    {

    }

    protected MemberNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }
}