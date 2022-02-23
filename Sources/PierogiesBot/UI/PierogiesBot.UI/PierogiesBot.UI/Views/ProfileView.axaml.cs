using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Shared.UI.IoC;
using Shared.UI.ViewModels.ProfileView;

namespace PierogiesBot.UI.Views;

public partial class ProfileView : ReactiveUserControl<ProfileViewModel>
{
    public ProfileView() : this(ServiceLocator.GetRequiredService<ProfileViewModel>()) {}
    public ProfileView(ProfileViewModel viewModel)
    {
        InitializeComponent();
        this.WhenActivated(d => { });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}