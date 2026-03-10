// using FluentValidation;
// using LanggoNew.Endpoints;
// using LanggoNew.Features.Dictionaries.AddDictionary;
// using LanggoNew.Models;
// using LanggoNew.Shared.Infrastructure.PasswordHashing;
// using Microsoft.AspNetCore.Http.HttpResults;
//
// namespace LanggoNew.Features.Authentication.Register;
//
// public static class Register
// {
//     public class Endpoint : IEndpoint
//     {
//         public record Request(
//             string Email, 
//             string Password,
//             string Username,
//             string? FullName,
//             string? Avatar,
//             string LearningLanguage,
//             string NativeLanguage);
//         
//         public class Validator : AbstractValidator<Request>
//         {
//             public Validator()
//             {
//                 RuleFor(x => x.Email).NotEmpty().EmailAddress();
//                 RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
//                 RuleFor(x => x.Username).NotEmpty();
//                 RuleFor(x => x.LearningLanguage).NotEmpty();
//                 RuleFor(x => x.NativeLanguage).NotEmpty();
//             }
//         }
//         
//         public void MapEndpoint(IEndpointRouteBuilder app)
//         {
//             app.MapPost("/register", Handler).WithTags("Authentication");
//         }
//
//         private static async Task<Results<Created, BadRequest>> Handler(
//             Request request,
//             AppDbContext context,
//             IPasswordHashingService passwordHashingService,
//             IValidator<Request> validator)
//         {
//             await validator.ValidateAndThrowAsync(request);
//             
//             var hashedPassword = passwordHashingService.HashPassword(request.Password);
//             
//             var user = new User
//             {
//                 Email = request.Email,
//                 Password = hashedPassword, 
//                 Username = request.Username,
//                 FullName = request.FullName ?? string.Empty,
//                 Avatar = request.Avatar ?? string.Empty,
//                 LearningLanguage = request.LearningLanguage,
//                 NativeLanguage = request.NativeLanguage
//             };
//             
//             context.Users.Add(user);
//             await context.SaveChangesAsync();
//             
//             return TypedResults.Created();
//         }
//     }
// }