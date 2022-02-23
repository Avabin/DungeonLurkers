using ReactiveUI;

namespace Shared.UI.Navigation.RoutableViewModel;

public interface IRoutableViewModelFactory
{
    T GetViewModel<T>() where T : IRoutableViewModel;
}