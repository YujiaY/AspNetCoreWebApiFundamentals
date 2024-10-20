using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo_Dev.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(IConfiguration configuration) : ControllerBase
{
    public class AuthenticationRequestBody
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
    {
        public int UserId { get; set; } = userId;
        public string UserName { get; set; } = userName;
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public string City { get; set; } = city;
    }
    
    private CityInfoUser? ValidateUserCredentials(string? requestBodyUsername, string? requestBodyPassword)
    {
        if (string.IsNullOrEmpty(requestBodyUsername) || string.IsNullOrEmpty(requestBodyPassword))
        {
            return null;
        }
        // Or work with a database.
        return new CityInfoUser(
            1,
            requestBodyUsername ?? "admin",
            "Jack",
            "Yuan",
            "Brisbane"
        );
    }
    
    [HttpPost("authenticate")]
    public ActionResult<string> Authenticate(
        AuthenticationRequestBody requestBody)
    {
        // Step 1: validate the username and password
        var user = ValidateUserCredentials(requestBody.Username, requestBody.Password);

        if (user == null)
        {
            return Unauthorized();
        }
        
        // Step 2: create a JWT token
        var securityKey = new SymmetricSecurityKey(
            Convert.FromBase64String(configuration["Authentication:SecretForKey"] ?? string.Empty));
        
        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);
        
        // The claims
        var claimsForToken = new List<Claim>();
        claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
        claimsForToken.Add(new Claim("given_name", user.FirstName));
        claimsForToken.Add(new Claim("family_name", user.LastName));
        claimsForToken.Add(new Claim("city", user.City));
        claimsForToken.Add(new Claim(ClaimTypes.Locality, user.City));
        claimsForToken.Add(new Claim(ClaimTypes.Name, user.UserName));
        claimsForToken.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName));
        claimsForToken.Add(new Claim(ClaimTypes.Country, "Australia"));
        // var claimsForToken = new[]
        // {
        //     new Claim(ClaimTypes.Name, user.UserName),
        //     new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        //     new Claim(ClaimTypes.GivenName, user.FirstName),
        //     new Claim(ClaimTypes.Surname, user.LastName),
        //     new Claim(ClaimTypes.Locality, user.City)
        // };

        var jwtSecurityToken = new JwtSecurityToken(
            configuration["Authentication:Issuer"],
            configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(24),
            signingCredentials
        );
        
        var tokenToReturn = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return Ok(tokenToReturn);
    }
}