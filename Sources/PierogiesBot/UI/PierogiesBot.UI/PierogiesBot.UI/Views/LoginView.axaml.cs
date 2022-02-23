using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Shared.UI.IoC;
using Shared.UI.ViewModels.LoginView;
using Splat;

namespace PierogiesBot.UI.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>, IEnableLogger
{
    public LoginView() : this(ServiceLocator.GetRequiredService<LoginViewModel>()) {}
    public LoginView(LoginViewModel viewModel)
    {
        AvaloniaXamlLoader.Load(this);
        ViewModel = viewModel;
        this.WhenActivated(d =>
        {
            
        });
    }
}