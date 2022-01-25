using System.ComponentModel.DataAnnotations;

namespace Shared.Features.Users;

public record CreateUserDto : ICreateDocumentDto
{
    public                string       UserName { get; set; } = "";
    [EmailAddress] public string       Email    { get; set; } = "";
    public                string       Password { get; set; } = "";
    public                List<string> Roles    { get; set; } = new();
}