using DocAssociados.Identity.Application.DTOs;
using DocAssociados.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DocAssociados.Identity.WebApi;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminsOnly")]
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
    [AllowAnonymous]
    public async Task<IActionResult> GetTokenAsync(TokenRequestDTO requestDto)
    {
        _log.LogInformation("Receiving request to login, e-mail:{0}", requestDto.Email);
        var result = await _service.GetTokenAsync(requestDto);

        if (result.IsAuthenticated)
        {
            _log.LogInformation("User authentication sucessufuly to e-mail: {RequestEmail}", requestDto.Email);

            HttpContext.Response.Cookies.Append(
            "refreshToken",
            result.RefreshToken!,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });

            return Ok(result);
        }
        else
        {
            _log.LogWarning("User authentication failed to e-mail: {RequestEmail}, error message: {ErrorMessage}",
                                                                                requestDto.Email, result.Message);

            return Unauthorized();
        }
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

    [HttpGet("refresh-token")]
    public async Task<ActionResult> RefreshToken()
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        _log.LogInformation("Receiving request to refresh token");
        var response = await _service.RefreshTokenAsync(refreshToken!);

        if (response.IsAuthenticated)
        {
            _log.LogInformation("Refresh token updated sucessufuly to e-mail: {RequestEmail}", response.Email);

            return Ok(response);
        }
        else
        {
            _log.LogWarning("Something went wrong with refresh token to e-mail: {RequestEmail} with error message: {ErrorMessage}",
                response.Email, response.Message);

            return Unauthorized();
        }
    }

    [HttpPost("update-password")]
    public async Task<ActionResult<bool>> UpdatePassword(UpdatePasswordDto updatePasswordDto)
    {
        return await _service.UpdatePasswordAsync(updatePasswordDto);
    }

    [HttpDelete("delete-user-auth/{id:Guid}")]
    public async Task DeleteUserIdentity(Guid id)
    {
        await _service.DeleteUserIdentityAsync(id);
    }

    [HttpGet("logout/{email}")]
    public async Task<IActionResult> Logout(string email)
    {
        HttpContext.Response.Cookies.Delete("refreshToken");

        await _service.RevokRefreshToken(email);

        return Ok(new { message = "Logout efetuado com sucesso." });
    }
}
