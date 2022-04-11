using System.Reactive.Linq;

namespace Shared.UI.Observables;

public static class ObservableExtensions
{
    public static IObservable<bool> Toggle(this IObservable<bool> o) => o.Select(x => !x);
}