using Shared.Features.Roles;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Identity.Features.Roles.Many;
using Shared.Persistence.Identity.Features.Roles.Single;

namespace Shared.Persistence.Identity.Features.Roles;

public class RoleFacade : DocumentOperationFacade<RoleDocument, string, FindRoleDto>
{
    public RoleFacade(
        ISingleRoleService singleSingleDocumentService,
        IManyRolesService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService)
    {
    }
}