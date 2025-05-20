using System.Security.Claims;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController(IUserCredentialService userCredentialService, IAuthenticationService authenticationService, IRefreshTokenService refreshTokenService, IUserService userService, IUserTypeService userTypeService) : ControllerBase
{
    [HttpPost("UpdateUserCredentials")]
    public ActionResult UpdateUserCredentials([FromBody] UserIdPassword parameters)
    {
        userCredentialService.UpdateUserCredentials(parameters.UserId, parameters.Password);
        
        return Ok();
    }
    
    [HttpPost("login")]
    public ActionResult<SignInResponse> Login([FromBody] UserCredentialsDto userCredentials)
    {
        var user = userCredentialService.GetUserByEmail(userCredentials.Email);
        var userType = userTypeService.GetUserTypeById(user.UserTypeId).Name;
        if (user.Id == 0 || !userCredentialService.ValidateUserCredentials(user.Id, userCredentials.Password)) 
        {
            return Unauthorized("Invalid credentials");
        }

        var accessToken = authenticationService.GenerateAccessToken(user);
        var refreshToken = authenticationService.GenerateRefreshToken();
        
        refreshTokenService.Store(
            user.Id, 
            refreshToken, 
            DateTime.UtcNow.AddDays(15)
        );
        
        return Ok(new SignInResponse(accessToken, refreshToken, user.Id, user.Name, userType));
    }

    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] TokenModel token)
        {
        if (string.IsNullOrEmpty(token.RefreshToken) || token.RefreshToken == "undefined")
        {
            return BadRequest("Invalid refresh token");
        }

        var principal = authenticationService.GetPrincipalFromExpiredToken(token.AccessToken);
        
        if (!int.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return BadRequest("Invalid access token");
        }
        
        if (!refreshTokenService.Validate(userId, token.RefreshToken))
        {
            refreshTokenService.Remove(userId);
            return BadRequest("Invalid or expired refresh token");
        }

        var user = userService.GetUserById(userId);
        
        if (user == new UserDto())
        {
            return BadRequest("User not found");
        }
        
        return Ok(new TokenModel(authenticationService.GenerateAccessToken(user), token.RefreshToken));
    }
    
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            refreshTokenService.Remove(userId);
        }
        
        return Ok();
    }
}

public class SignInResponse(string accessToken, string refreshToken, int userId, string name, string userType = "user")
{
    public string AccessToken { get; set; } = accessToken;
    public string RefreshToken { get; set; } = refreshToken;
    public int UserId { get; set; } = userId;
    public string Name { get; set; } = name;
    public string UserType { get; set; } = userType;
}

public class TokenModel(string accessToken, string refreshToken)
{
    public string AccessToken { get; } = accessToken;
    public string RefreshToken { get;} = refreshToken;
}

public class UserIdPassword(int userId, string password)
{
    public int UserId { get; } = userId;
    public string Password { get; } = password;
}