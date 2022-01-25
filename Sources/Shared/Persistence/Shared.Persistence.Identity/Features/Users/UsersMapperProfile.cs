using Shared.Features;
using Shared.Features.Users;

namespace Shared.Persistence.Identity.Features.Users;

public class UsersMapperProfile : DtoMapperProfile<UserDto, CreateUserDto, UpdateUserDto, UserDocument>
{
}