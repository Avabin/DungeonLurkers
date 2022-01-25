using System.Runtime.Serialization;

namespace TheDungeonGuide.Persistence.Sessions.Features.Exceptions;

public class CharacterNotFoundException : Exception
{
    public CharacterNotFoundException(string characterId) : base($"Character with ID {characterId} not found")
    {

    }

    protected CharacterNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }
}