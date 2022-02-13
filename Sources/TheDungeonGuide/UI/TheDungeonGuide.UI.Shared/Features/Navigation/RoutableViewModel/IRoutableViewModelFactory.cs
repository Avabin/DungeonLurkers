using ReactiveUI;

namespace TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;

public interface IRoutableViewModelFactory
{
    T GetViewModel<T>() where T : IRoutableViewModel;
}