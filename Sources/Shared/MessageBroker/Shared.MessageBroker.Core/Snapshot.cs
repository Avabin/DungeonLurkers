namespace Shared.MessageBroker.Core;

public record Snapshot<T>(T? Previous, T? Current);
public static class Snapshot
{
    public static Snapshot<T> Of<T>(T? previous = default, T? current = default) => new(previous, current);
};