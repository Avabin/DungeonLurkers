namespace Shared.Persistence.Core.Features.Exceptions;

public class DocumentNotFoundException : Exception
{
    public DocumentNotFoundException(string message) : base(message)
    {
        
    }
}