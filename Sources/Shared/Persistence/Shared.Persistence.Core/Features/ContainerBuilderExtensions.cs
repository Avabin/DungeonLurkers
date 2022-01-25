using Autofac;
using Shared.Persistence.Core.Features.Documents.Many;

namespace Shared.Persistence.Core.Features;

public static class ContainerBuilderExtensions
{
    public static void AddPersistenceCore(this ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(DocumentOperationFacade<,,>)).AsImplementedInterfaces();
    }
}