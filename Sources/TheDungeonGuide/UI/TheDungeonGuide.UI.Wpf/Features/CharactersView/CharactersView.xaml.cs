using System.Windows.Controls;
using ReactiveUI;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;
using TheDungeonGuide.UI.ViewModels.Features.CharactersView;

namespace TheDungeonGuide.UI.Wpf.Features.CharactersView;

public partial class CharactersView
{
    public CharactersView(CharactersViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
    }
}