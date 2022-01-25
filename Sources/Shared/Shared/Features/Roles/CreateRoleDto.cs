namespace Shared.Features.Roles;

public record CreateRoleDto : ICreateDocumentDto
{
    public string Name { get; set; } = "";
}