using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Shared.Features.Users;

namespace Shared.UI.UserStore;

public class AppUserStore : IUserStore
{
    private readonly ISubject<UserDto?>   _userInfoSubject;
    public           IObservable<UserDto> UserInfoObservable => _userInfoSubject.WhereNotNull().AsObservable();

    public AppUserStore()
    {
        _userInfoSubject = new BehaviorSubject<UserDto?>(null);
    }
    
    public void PublishUserInfo(UserDto userInfo)
    {
        _userInfoSubject.OnNext(userInfo);
    }
}