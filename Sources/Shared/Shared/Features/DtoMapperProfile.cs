using AutoMapper;

namespace Shared.Features;

public class DtoMapperProfile<TFindDto, TCreateDto, TUpdateDto, TDocument> : Profile
    where TFindDto : IDocumentDto<string>
    where TUpdateDto : IUpdateDocumentDto
    where TCreateDto : ICreateDocumentDto
{
    public DtoMapperProfile()
    {
        CreateMap<TFindDto, TDocument>().ReverseMap();
        CreateMap<TUpdateDto, TDocument>().ReverseMap();
        CreateMap<TCreateDto, TDocument>().ReverseMap();
    }
}