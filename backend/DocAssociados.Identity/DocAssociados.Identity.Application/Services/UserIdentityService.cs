using AutoMapper;
using DocAssociados.Identity.Application.DTOs;
using DocAssociados.Identity.Application.Interfaces;
using DocAssociados.Identity.Domain.Entity;
using DocAssociados.Identity.Domain.Interfaces;

namespace DocAssociados.Identity.Application.Services;

public class UserIdentityService : IUserIdentityService
{
    private IUserIdentityRepository _identityRepository;
    private IMapper _mapper;

    public UserIdentityService(IUserIdentityRepository userRepository, IMapper mapper)
    {
        _identityRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<AuthenticationDTO> GetTokenAsync(TokenRequestDTO requestDto)
    {
        var requestEntity = _mapper.Map<TokenRequest>(requestDto);
        var authentication = await _identityRepository.GenerateAccessTokenAsync(requestEntity);
        return _mapper.Map<AuthenticationDTO>(authentication);
    }

    public async Task<AuthenticationDTO> RefreshTokenAsync(string token)
    {
        var authEntity = await _identityRepository.RefreshTokenAsync(token);
        return _mapper.Map<AuthenticationDTO>(authEntity);
    }

    public async Task<ResponseDTO> RegisterAsync(UserIdentityDTO userDto)
    {
        var response = new ResponseDTO();
        var userEntity = _mapper.Map<UserIdentity>(userDto);
        var result = await _identityRepository.RegisterAsync(userEntity);

        if (result.GetSuccess())
        {
            var requestToken = new TokenRequest(userEntity.Email, userEntity.Password);
            var authEntity = await _identityRepository.GenerateAccessTokenAsync(requestToken);

            response.Id = result.Id;
            response.Email = result.Email;
            response.Token = authEntity.Token;
            response.RefreshToken = authEntity.RefreshToken;
            response.Success = true;
            response.CreatedAt = result.CreatedAt;

            return response;
        }

        foreach (var erro in result.GetErrors())
        {
            response.Errors.Add(erro);
        }

        return response;
    }
}
