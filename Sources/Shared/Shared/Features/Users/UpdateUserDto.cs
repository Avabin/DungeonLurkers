using System.ComponentModel.DataAnnotations;

namespace Shared.Features.Users;

public record UpdateUserDto : IUpdateDocumentDto
{
    public                string? UserName { get; set; }
    [EmailAddress] public string? Email    { get; set; }

    public IEnumerable<string> Roles { get; set; } = new List<string>();
}