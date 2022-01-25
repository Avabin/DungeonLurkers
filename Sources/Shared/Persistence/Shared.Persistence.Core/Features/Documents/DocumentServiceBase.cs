﻿using AutoMapper;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Core.Features.Documents;

public abstract class DocumentServiceBase<TDocument, TId> where TDocument : IDocument<TId>
{

    protected DocumentServiceBase(IRepository<TDocument, TId> repository, IMapper mapper)
    {
        Repository = repository;
        Mapper     = mapper;
    }
    protected IRepository<TDocument, TId> Repository { get; }
    protected IMapper                     Mapper     { get; }
}