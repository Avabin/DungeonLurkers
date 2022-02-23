using System.Reactive;
using ReactiveUI;

namespace Shared.UI.HostScreen;

public interface IHostScreenViewModel : IScreen
{
    IObservable<IRoutableViewModel>           Navigate<T>() where T : IRoutableViewModel;
    IObservable<IRoutableViewModel?>          NavigateBack();
    ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateCommand<T>() where T : IRoutableViewModel;
    ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateAndResetCommand<T>() where T : IRoutableViewModel;
}