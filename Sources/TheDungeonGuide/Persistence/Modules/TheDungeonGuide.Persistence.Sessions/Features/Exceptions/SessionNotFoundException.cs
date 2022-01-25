using System.Runtime.Serialization;

namespace TheDungeonGuide.Persistence.Sessions.Features.Exceptions;

[Serializable]
public class SessionNotFoundException : Exception
{
    public SessionNotFoundException(string sessionId) : base($"Session with id {sessionId} not found")
    {

    }

    protected SessionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }
}