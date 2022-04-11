using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Shared.UI.IoC;
using Shared.UI.ViewModels.LoginView;
using Splat;

namespace TheDungeonGuide.UI.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>, IEnableLogger
{
    public TextBox Username => this.FindControl<TextBox>("UsernameTextBox");
    public TextBox Password => this.FindControl<TextBox>("PasswordTextBox");
    public LoginView() : this(ServiceLocator.GetRequiredService<LoginViewModel>()) {}
    public LoginView(LoginViewModel viewModel)
    {
        ViewModel = viewModel;
        AvaloniaXamlLoader.Load(this);
        this.WhenActivated(d =>
        {
            
        });
    }
}