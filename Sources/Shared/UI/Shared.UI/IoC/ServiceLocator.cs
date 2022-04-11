using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace Shared.UI.IoC;

public static class ServiceLocator
{
    public static IServiceProvider? Instance        { get; set; }
    public static T?                GetService<T>() => Locator.Current.GetService<T>();

    public static object? GetService(Type serviceType) => Locator.Current.GetService(serviceType);
    public static T               GetRequiredService<T>() where T : notnull => GetService<T>() ?? throw new InvalidOperationException($"Service {typeof(T).FullName} is not registered");
}