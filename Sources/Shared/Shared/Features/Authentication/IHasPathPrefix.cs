using RestEase;

namespace Shared.Features.Authentication;

public interface IHasPathPrefix
{
    [Path("PathPrefix")] string PathPrefix { get; set; }
}