using System.Windows.Controls;
using TheDungeonGuide.UI.ViewModels.Features.SessionsView;

namespace TheDungeonGuide.UI.Wpf.Features.SessionsView;

public partial class SessionsView
{
    public SessionsView(SessionsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}