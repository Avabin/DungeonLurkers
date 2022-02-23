using Shared.UI.ViewModels.LoginView;
using Shared.UI.ViewModels.MainView;
using Shared.UI.ViewModels.ProfileView;
using Splat;

namespace TheDungeonGuide.UI.Views;

public static class DesignData
{
    public static LoginViewModel   LoginViewModel   => Vm<LoginViewModel>();
    public static MainViewModel    MainViewModel    => Vm<MainViewModel>();
    public static ProfileViewModel ProfileViewModel => Vm<ProfileViewModel>();


    private static T Vm<T>() => Locator.GetLocator().GetService<T>()!;
}