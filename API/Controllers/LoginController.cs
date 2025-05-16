using System.Security.Claims;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController(IUserCredentialService userCredentialService, ITokenService tokenService, IInMemoryTokenService tokens) : ControllerBase
{
    [HttpGet("ValidateUserCredentials")]
    public ActionResult<bool> ValidateUserCredentials([FromBody] UserCredentialsDto userCredentials)
    {
        return Ok(userCredentialService.ValidateUserCredentials(userCredentials));
    }

    [HttpGet("GetUserDataByEmail")]
    public ActionResult<UserDto> GetUserDataByEmail(string email)
    {
        return Ok(userCredentialService.GetUserByEmail(email));
    }
    [HttpPost("login")]
    public ActionResult<SignInResponse> Login([FromBody] UserCredentialsDto userCredentials)
    {
        // Validate user credentials
        var user = userCredentialService.GetUserByEmail(userCredentials.Email);
        if (user.Id == 0 || !userCredentialService.ValidateUserCredentials(userCredentials)) 
        {
            return Unauthorized("Invalid credentials");
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        
        // Store refresh token in memory
        tokens.StoreRefreshToken(
            user.Id, 
            refreshToken, 
            DateTime.UtcNow.AddMinutes(10)
        );
        
        var response = new SignInResponse(accessToken, refreshToken, user.Id, user.Name);

        
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel token)
        {
        if (string.IsNullOrEmpty(token.RefreshToken) || token.RefreshToken == "undefined")
        {
            return BadRequest("Invalid refresh token");
        }

        var principal = tokenService.GetPrincipalFromExpiredToken(token.AccessToken);
        
        if (!int.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return BadRequest("Invalid access token");
        }
        
        // Validate refresh token from memory
        if (!tokens.ValidateRefreshToken(userId, token.RefreshToken))
        {
            return BadRequest("Invalid or expired refresh token");
        }

        var user = userCredentialService.GetUserByEmail(principal.FindFirst(ClaimTypes.Email)?.Value);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var newToken = new TokenModel();

        newToken.AccessToken = tokenService.GenerateAccessToken(user);
        newToken.RefreshToken = tokenService.GenerateRefreshToken();

        // Update refresh token in memory
        /*tokens.StoreRefreshToken(
            userId, 
            newRefreshToken, 
            DateTime.UtcNow.AddSeconds(30)
        );*/

        return Ok(newToken);
    }
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            // Remove refresh token from memory
            tokens.RemoveRefreshToken(userId);
        }
        
        return Ok();
    }
}
public class SignInResponse(string accessToken, string refreshToken, int userId, string name)
{
    public string AccessToken { get; set; } = accessToken;
    public string RefreshToken { get; set; } = refreshToken;
    public int UserId { get; set; } = userId;
    public string Name { get; set; } = name;
}
public class TokenModel
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}