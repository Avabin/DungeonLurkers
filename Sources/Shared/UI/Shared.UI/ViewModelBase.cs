using System.Reactive;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using Shared.UI.HostScreen;

namespace Shared.UI;

public abstract class ViewModelBase : ReactiveValidationObject, IRoutableViewModel
{
    protected ViewModelBase(IHostScreenViewModel hostScreenViewModel)
    {
        HostScreenViewModel = hostScreenViewModel;
    }

    public abstract string UrlPathSegment { get; }
    public IScreen HostScreen => HostScreenViewModel;
    public IHostScreenViewModel HostScreenViewModel { get; }

    protected IObservable<IRoutableViewModel> Navigate<T>() where T : ViewModelBase => HostScreenViewModel.Navigate<T>();

    protected IObservable<IRoutableViewModel?>          NavigateBack()                                             => HostScreenViewModel.NavigateBack();
    protected ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateCommand<T>() where T : ViewModelBase => HostScreenViewModel.CreateNavigateCommand<T>();
    protected ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateCommand(IRoutableViewModel viewModel)        => ReactiveCommand.CreateFromObservable(() => HostScreenViewModel.Router.Navigate.Execute(viewModel));
}