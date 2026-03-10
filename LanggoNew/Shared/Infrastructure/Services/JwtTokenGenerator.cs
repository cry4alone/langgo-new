using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LanggoNew.Models;
using Microsoft.IdentityModel.Tokens;

namespace LanggoNew.Shared.Infrastructure.Services;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(User user);
}

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    public string GenerateJwtToken(User user)
    {
        var secret = configuration["Jwt:Secret"];
        if(string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("JWT secret is not configured.");
             
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);
             
        var claims = new []
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
             
        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}