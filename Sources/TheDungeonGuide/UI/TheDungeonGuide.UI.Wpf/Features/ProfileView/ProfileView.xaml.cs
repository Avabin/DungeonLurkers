using System;
using System.Windows.Controls;
using ReactiveUI;
using TheDungeonGuide.UI.ViewModels.Features.ProfileView;

namespace TheDungeonGuide.UI.Wpf.Features.ProfileView;

public partial class ProfileView
{
    public ProfileView(ProfileViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;

        this.WhenActivated(ConfigureSubscriptions);
    }

    private void ConfigureSubscriptions(Action<IDisposable> d)
    {
        d(this.Bind(ViewModel, vm => vm.Username, v => v.UsernameLabel.Content));
        
        
    }
}