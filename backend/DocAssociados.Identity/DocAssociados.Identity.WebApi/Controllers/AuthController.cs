using DocAssociados.Identity.Application.DTOs;
using DocAssociados.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocAssociados.Identity.WebApi;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly IUserIdentityService _service;
    private readonly ILogger<AuthController> _log;
    public AuthController(IUserIdentityService userService, ILogger<AuthController> log)
    {
        _service = userService;
        _log = log;
    }

    [HttpPost("login")]
    public async Task<IActionResult> GetTokenAsync(TokenRequestDTO requestDto)
    {
        _log.LogInformation("Receiving request to login, e-mail:{0}", requestDto.Email);
        var result = await _service.GetTokenAsync(requestDto);

        if (result.IsAuthenticated)
            _log.LogInformation("User authentication sucessufuly to e-mail: {RequestEmail}", requestDto.Email);
        else
            _log.LogWarning("User authentication failed to e-mail: {RequestEmail}, error message: {ErrorMessage}",
                requestDto.Email, result.Message);

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(UserIdentityDTO userInfo)
    {
        _log.LogInformation("Receiving new user registration, e-mail:{UserEmail}", userInfo.Email);
        var result = await _service.RegisterAsync(userInfo);

        if (result.Success)
        {
            _log.LogInformation("(User registred successufuly, id:{UserId} e-mail:{UserEmail}", result.Id, userInfo.Email);
            return Ok(result);
        }

        _log.LogWarning("(Something went wrong with user registration errors: {ResultError}", result.Errors);
        return BadRequest(result);
    }

    [HttpPost("refreshtoken")]
    public async Task<ActionResult> RefreshToken(string refreshToken)
    {
        _log.LogInformation("Receiving request to refresh token");
        var response = await _service.RefreshTokenAsync(refreshToken!);

        if (response.IsAuthenticated)
            _log.LogInformation("Refresh token updated sucessufuly to e-mail: {RequestEmail}", response.Email);
        else
            _log.LogWarning("Something went wrong with refresh token to e-mail: {RequestEmail} with error message: {ErrorMessage}",
                response.Email, response.Message);


        return Ok(response);
    }
}
