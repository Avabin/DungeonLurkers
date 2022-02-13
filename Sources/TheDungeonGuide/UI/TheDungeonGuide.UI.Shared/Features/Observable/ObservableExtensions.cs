﻿using System.Reactive.Linq;

namespace TheDungeonGuide.UI.Shared.Features.Observable;

public static class ObservableExtensions
{
    public static IObservable<bool> Toggle(this IObservable<bool> o) => o.Select(x => !x);
}