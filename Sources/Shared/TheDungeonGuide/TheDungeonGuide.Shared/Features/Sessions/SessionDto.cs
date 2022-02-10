using System.ComponentModel.DataAnnotations;
using Shared.Features;

namespace TheDungeonGuide.Shared.Features.Sessions;

public record SessionDto : IDocumentDto<string>
{
    public string Title { get; set; } = "";
    [RegularExpression(@"/^[a-f\d]{24}$/i", ErrorMessage = "Must be a valid object ID")]
    public string GameMasterId { get;               set; } = "";
    public IEnumerable<string> PlayersIds    { get; set; } = new List<string>();
    public IEnumerable<string> CharactersIds { get; set; } = new List<string>();
    [RegularExpression(@"/^[a-f\d]{24}$/i", ErrorMessage = "Must be a valid object ID")]
    public string Id { get; set; } = "";
}