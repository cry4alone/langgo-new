using FluentValidation;
using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Games.CreateGame;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("games/room",
            async (
                Command cmd,
                ISender sender,
                IValidator<Command> validator,
                CancellationToken cancellationToken) =>
        {
            await validator.ValidateAndThrowAsync(cmd, cancellationToken);
            
            var response = await sender.Send(cmd, cancellationToken);
            return TypedResults.Ok(response);
        }).WithTags("Games").RequireAuthorization();
    }
}