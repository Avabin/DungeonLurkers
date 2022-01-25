using MongoDB.Bson;
using Shared.Persistence.Core.Features.Documents;

namespace TheDungeonGuide.Persistence.Sessions;

public record SessionDocument : DocumentBase<string>
{
    public SessionDocument(string title, string gameMasterId, string? id = null) : base(id ?? ObjectId.GenerateNewId().ToString())
    {
        Title        = title;
        GameMasterId = gameMasterId;
    }

    public SessionDocument() : this("", "")
    {

    }
    public string       Title         { get; init; }
    public List<string> PlayersIds    { get; init; } = new();
    public List<string> CharactersIds { get; init; } = new();
    public string       GameMasterId  { get; init; }
}