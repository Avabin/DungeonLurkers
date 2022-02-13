using Microsoft.Extensions.DependencyInjection;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;

namespace TheDungeonGuide.UI.Shared.Features.Navigation
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add custom implementation of <see cref="INavigationService"/>
        /// </summary>
        /// <param name="collection">Service collection</param>
        /// <typeparam name="TNavigation">Type of implementation of <see cref="INavigationService"/></typeparam>
        /// <returns>collection</returns>
        public static IServiceCollection AddNavigation<TNavigation>(this IServiceCollection collection)
            where TNavigation : class, INavigationService
            => collection.AddNavigationCore<TNavigation>();

        /// <summary>
        /// Add <see cref="NavigationService"/>
        /// </summary>
        /// <param name="collection">Service collection</param>
        /// <returns>collection</returns>
        public static IServiceCollection AddNavigation(this IServiceCollection collection)
            => collection.AddNavigationCore<NavigationService>();

        private static IServiceCollection AddNavigationCore<T>(this IServiceCollection services)
            where T : class, INavigationService =>
            services
               .AddSingleton<INavigationService, T>()
               .AddTransient<IRoutableViewModelFactory, RoutableViewModelFactory>();
    }
}