using AutoMapper;
using DocAssociados.Identity.Application.DTOs;
using DocAssociados.Identity.Domain.Entity;

namespace DocAssociados.Identity.Application.Mappings;

public class DomainToDTOMappingProfile : Profile
{
    public DomainToDTOMappingProfile()
    {
        CreateMap<UserIdentity, UserIdentityDTO>().ReverseMap();
        CreateMap<Authentication, AuthenticationDTO>().ReverseMap();
        CreateMap<TokenRequest, TokenRequestDTO>().ReverseMap();
        CreateMap<UserToRole, UserToRoleDTO>().ReverseMap();
    }
}
