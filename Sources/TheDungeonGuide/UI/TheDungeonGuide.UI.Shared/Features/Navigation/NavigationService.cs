using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace TheDungeonGuide.UI.Shared.Features.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _container;

        public NavigationService(IServiceProvider container)
        {
            _container = container;
        }

        public RoutingState Router { get; } = new();

        public IObservable<IRoutableViewModel> NavigateTo<T>() where T : IRoutableViewModel
        {
            var next = _container.GetRequiredService<T>();
            return Router.Navigate.Execute(next);
        }

        public IObservable<IRoutableViewModel> NavigateToAndReset<T>() where T : IRoutableViewModel
        {
            var next = _container.GetRequiredService<T>();
            return Router.NavigateAndReset.Execute(next);
        }

        public IObservable<IRoutableViewModel?> NavigateBack() => Router.NavigateBack.Execute();
    }
}