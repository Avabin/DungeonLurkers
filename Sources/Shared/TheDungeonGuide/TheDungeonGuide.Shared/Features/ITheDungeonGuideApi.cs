using Shared.Features.Users;
using TheDungeonGuide.Shared.Features.Characters;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Shared.Features;

public interface ITheDungeonGuideApi : ICharactersApi, ISessionsApi, IUsersApi
{
    
}