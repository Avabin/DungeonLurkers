using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Features;

namespace TheDungeonGuide.Shared.Features.Sessions;

public record CreateSessionDto : ICreateDocumentDto
{
    [Required] public string Title { get; set; } = "";
    [RegularExpression(@"^[a-f\d]{24}$", ErrorMessage = "Must be a valid object ID")]
    public string GameMasterId { get;                          set; } = "";
    [Required] public IEnumerable<string> PlayersIds    { get; set; } = new List<string>();
    [Required] public IEnumerable<string> CharactersIds { get; set; } = new List<string>();
}