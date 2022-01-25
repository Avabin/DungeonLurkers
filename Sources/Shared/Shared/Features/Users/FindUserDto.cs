using System.ComponentModel.DataAnnotations;

namespace Shared.Features.Users;

public record UserDto : IDocumentDto<string>
{
    public                string UserName { get; set; } = "";
    [EmailAddress] public string Email    { get; set; } = "";

    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string              Id    { get; set; } = "";
}