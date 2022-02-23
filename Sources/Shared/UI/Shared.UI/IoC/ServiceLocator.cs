using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace Shared.UI.IoC;

public static class ServiceLocator
{
    public static IServiceProvider? Instance        { get; set; }
    public static T?                GetService<T>() => Instance == null ? Locator.GetLocator().GetService<T>() : Instance.GetService<T>();

    public static object? GetService(Type serviceType) => Instance == null ? Locator.GetLocator().GetService(serviceType) : Instance.GetService(serviceType);
    public static T               GetRequiredService<T>() where T : notnull => GetService<T>()!;
}