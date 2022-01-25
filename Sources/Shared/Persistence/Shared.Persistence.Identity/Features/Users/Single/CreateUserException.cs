using System.Runtime.Serialization;

namespace Shared.Persistence.Identity.Features.Users.Single;

public class CreateUserException : Exception
{

    public CreateUserException(IReadOnlyCollection<string> causes) : base(string.Join(",", causes)) => Causes = causes;

    protected CreateUserException(SerializationInfo info, StreamingContext context) : base(info, context) =>
        Causes = new List<string>();
    public IReadOnlyCollection<string> Causes { get; }
}