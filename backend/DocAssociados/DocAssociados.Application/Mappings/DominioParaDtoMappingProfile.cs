using AutoMapper;
using DocAssociados.Application.DTOs;
using DocAssociados.Domain.Entities;
using DocAssociados.Service.Application.DTOs;
using DocAssociados.Service.Domain.EntitiesSummary;

namespace DocAssociados.Application.Mappings;

public class DominioParaDtoMappingProfile : Profile
{
    public DominioParaDtoMappingProfile()
    {
        CreateMap<Associado, AssociadoDto>()
            .ForMember(dest => dest.EnderecoDto, opt => opt.MapFrom(src => src.Endereco))
            .ForPath(dest => dest.FichaAssociadoDto.FichaAssociacaoUploadUrl,
                            opt => opt.MapFrom(src => src.FichaAssociacaoUploadUrl))
            .ForPath(dest => dest.TermoAdesaoDto.TermoAdesaoUploadUrl, 
                            opt => opt.MapFrom(src => src.TermoDeAdessaoUploadUrl))
            .ForPath(dest => dest.RequerimentoJudicialDto.UrlDoRequerimento, 
                            opt => opt.MapFrom(src => src.RequerimentoJudicialUrl))
            .ReverseMap();

        CreateMap<Endereco, EnderecoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Associado>, ResultadoPaginado<AssociadoDto>>().ReverseMap();
        CreateMap<AssociadoResumido, AssociadoResumidoDto>().ReverseMap();
    }
}
