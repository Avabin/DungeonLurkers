using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.UI.HostScreen;
using Shared.UI.Navigation.RoutableViewModel;

namespace Shared.UI.ViewModels.HostScreen;

public abstract class HostScreenViewModelBase : ReactiveObject, IHostScreenViewModel
{
    private readonly IRoutableViewModelFactory _routableViewModelFactory;
    public RoutingState Router { get; }
    public ReactiveCommand<Unit, IRoutableViewModel?> NavigateBackCommand { get; }
    [Reactive] public string? Url { get; set; }

    protected HostScreenViewModelBase(ILogger<HostScreenViewModelBase> logger,
        IRoutableViewModelFactory routableViewModelFactory)
    {
        _routableViewModelFactory = routableViewModelFactory;
        Router = new RoutingState();

        // can go back only if we are not on the first page
        var canGoBack = this.WhenAnyValue(x => x.Router.NavigationStack.Count).Select(count => count > 1);
        NavigateBackCommand =
            ReactiveCommand.CreateFromObservable(() => Router.NavigateBack.Execute(Unit.Default), canGoBack);

        Router.CurrentViewModel
            .Select(x => x?.UrlPathSegment ?? "?")
            .Do(s => logger.LogDebug("CurrentView: {CurrentViewUrlSegment}", s))
            .Subscribe();

        Router.CurrentViewModel
            .WhereNotNull()
            .Select(_ => Router.NavigationStack.Select(x => x.UrlPathSegment))
            .Select(paths => string.Join("/", paths))
            .BindTo(this, vm => vm.Url);
    }

    public IObservable<IRoutableViewModel> Navigate<T>() where T : IRoutableViewModel
    {
        var routableViewModel = _routableViewModelFactory.GetViewModel<T>();
        return Router.Navigate.Execute(routableViewModel);
    }

    public IObservable<IRoutableViewModel?> NavigateBack() => NavigateBackCommand.Execute();

    public ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateCommand<T>() where T : IRoutableViewModel =>
        ReactiveCommand.CreateFromObservable(Navigate<T>);

    public ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateAndResetCommand<T>() where T : IRoutableViewModel =>
        ReactiveCommand.CreateFromObservable(() =>
            Router.NavigateAndReset.Execute(_routableViewModelFactory.GetViewModel<T>()));
}