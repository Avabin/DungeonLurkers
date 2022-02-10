using Shared.Features;
using Shared.Persistence.Core.Features.Documents;

namespace Shared.MessageBroker.Persistence;

public record Document(string Id) : IDocument<string>;

