namespace Shared.Persistence.Core.Features.Documents;

public abstract record DocumentBase<T>(T Id) : IDocument<T>
{
}