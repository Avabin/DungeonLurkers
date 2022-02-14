using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using ReactiveUI;
using TheDungeonGuide.UI.ViewModels.Features.LoginView;

namespace TheDungeonGuide.UI.Wpf.Features.LoginView;

public partial class LoginView
{
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        
        ViewModel = viewModel;

        this.WhenActivated(ConfigureSubscriptions);
    }

    private void ConfigureSubscriptions(Action<IDisposable> d)
    {
        var passwordObservable = PasswordBox.Events().PasswordChanged.Select(x => (x.Source as PasswordBox)?.Password).WhereNotNull();
        d(this.Bind(ViewModel, vm => vm.Username, v => v.Username.Text));
        d(this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.LoginButton, passwordObservable));
    }
}