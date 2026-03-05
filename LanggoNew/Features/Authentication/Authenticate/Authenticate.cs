using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using LanggoNew.Endpoints;
using LanggoNew.Models;
using LanggoNew.Shared.Infrastructure.PasswordHashing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace LanggoNew.Features.Authentication.Authenticate;

public class Authenticate
{
    public class Endpoint : IEndpoint
    {
        public record Request(string Email, string Password);
        public record Response(string Token);
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("auth", Handler).WithTags("Authentication");
        }
        
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            }
        }

        public static async Task<Results<Ok<Response>, BadRequest<string>>> Handler(
            Request request,
            AppDbContext context,
            IPasswordHashingService passwordHashingService,
            IConfiguration configuration,
            IValidator<Request> validator)
        {
            await validator.ValidateAndThrowAsync(request);
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return TypedResults.BadRequest("Invalid email or password.");
            
            var isPasswordValid = passwordHashingService.VerifyHashedPassword(user.Password, request.Password);
            if(!isPasswordValid) return TypedResults.BadRequest("Invalid email or password.");
            
            var token = GenerateJwtToken(user, configuration);
            
            var response = new Response(token);
            
            return TypedResults.Ok(response);
        }

        private static string GenerateJwtToken(User user, IConfiguration configuration)
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
}