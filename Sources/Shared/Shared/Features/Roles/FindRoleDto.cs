namespace Shared.Features.Roles;

public record FindRoleDto : IDocumentDto<string>
{
    public FindRoleDto(string id) => Id = id;

    public string Id { get; }
}