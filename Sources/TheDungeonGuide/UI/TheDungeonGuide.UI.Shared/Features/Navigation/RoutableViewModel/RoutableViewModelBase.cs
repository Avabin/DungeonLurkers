using System.Reactive;
using ReactiveUI;
using TheDungeonGuide.UI.Shared.Features.HostScreen;

namespace TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;

public abstract class RoutableViewModelBase : ReactiveObject, IRoutableViewModel
{
    protected RoutableViewModelBase(IHostScreenViewModel hostScreenViewModel)
    {
        HostScreenViewModel = hostScreenViewModel;
    }

    public abstract string UrlPathSegment { get; }
    public IScreen HostScreen => HostScreenViewModel;
    public IHostScreenViewModel HostScreenViewModel { get; }

    protected IObservable<IRoutableViewModel> Navigate<T>() where T : RoutableViewModelBase => HostScreenViewModel.Navigate<T>();

    protected IObservable<IRoutableViewModel?> NavigateBack() => HostScreenViewModel.NavigateBack();
    protected ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateCommand<T>() where T : RoutableViewModelBase => HostScreenViewModel.CreateNavigateCommand<T>();
}