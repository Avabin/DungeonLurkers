using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;

namespace TheDungeonGuide.UI.Shared.Features.HostScreen;

public abstract class HostScreenViewModelBase : ReactiveObject, IHostScreenViewModel
{
    private readonly IRoutableViewModelFactory _routableViewModelFactory;
    public RoutingState                               Router                     { get; }
    public ReactiveCommand<Unit, IRoutableViewModel?> NavigateBackCommand        { get; }
    [Reactive] public string? Url { get; set; }

    protected HostScreenViewModelBase(ILogger<HostScreenViewModelBase> logger, IRoutableViewModelFactory routableViewModelFactory)
    {
        _routableViewModelFactory = routableViewModelFactory;
        Router = new RoutingState();

        // can go back only if we are not on the first page
        var canGoBack = this.WhenAnyValue(x => x.Router.NavigationStack.Count).Select(count => count > 0);
        NavigateBackCommand = ReactiveCommand.CreateFromObservable(() => Router.NavigateBack.Execute(Unit.Default), canGoBack);

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

    public IObservable<IRoutableViewModel> Navigate<T>() where T : IRoutableViewModel => 
        Router.Navigate.Execute(_routableViewModelFactory.GetViewModel<T>());

    public IObservable<IRoutableViewModel?> NavigateBack() => NavigateBackCommand.Execute();

    public ReactiveCommand<Unit, IRoutableViewModel> CreateNavigateCommand<T>() where T : IRoutableViewModel =>
        ReactiveCommand.CreateFromObservable(Navigate<T>);
}