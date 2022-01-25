using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Features;

namespace TheDungeonGuide.Shared.Features.Sessions;

public record UpdateSessionDto : IUpdateDocumentDto
{
    public string? Title { get; set; } = null;
    [RegularExpression(@"/^[a-f\d]{24}$/i", ErrorMessage = "Must be a valid object ID")]
    public string? GameMasterId { get;              set; } = null;
    public IEnumerable<string> MembersIds    { get; set; } = new List<string>();
    public IEnumerable<string> CharactersIds { get; set; } = new List<string>();
}