namespace Shared.Features;

public interface IDocumentDto<out T>
{
    T Id { get; }
}