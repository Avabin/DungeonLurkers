namespace Shared.Features;

/// <summary>
///     Generic document interface with only Id
/// </summary>
/// <typeparam name="T">Type of Id</typeparam>
public interface IDocument<out T>
{
    T Id { get; }
}