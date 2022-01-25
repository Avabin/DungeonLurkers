using MongoDB.Bson;
using Shared.Persistence.Core.Features.Documents;

namespace TheDungeonGuide.Persistence.Characters;

public record CharacterDocument : DocumentBase<string>
{
    public CharacterDocument(string ownerId = "", string? id = null, string name = "") : base(id ?? ObjectId.GenerateNewId().ToString())
    {
        OwnerId = ownerId;
        Name    = name;
    }
    public string Name { get; init; }

    public string OwnerId { get; init; }
}