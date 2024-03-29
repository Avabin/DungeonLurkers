﻿using Shared.Features;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Documents.Many;

namespace Shared.Persistence.Mongo.Features.Database.Documents;

public interface IMongoDocumentOperationFacade<TDocument, TFindDocDto>
    : IDocumentOperationFacade<TDocument, string, TFindDocDto>
    where TFindDocDto : IDocumentDto<string>
    where TDocument : IDocument<string>
{
}