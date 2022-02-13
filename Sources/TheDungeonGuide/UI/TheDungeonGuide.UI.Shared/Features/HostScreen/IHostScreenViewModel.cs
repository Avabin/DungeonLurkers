using System.Reactive;
using ReactiveUI;

namespace TheDungeonGuide.UI.Shared.Features.HostScreen;

public interface IHostScreenViewModel : IScreen
{
    IObservable<IRoutableViewModel> Navigate<T>() where T : IRoutableViewModel;
    IObservable<IRoutableViewModel?> NavigateBack();
    ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateCommand<T>() where T : IRoutableViewModel;
}