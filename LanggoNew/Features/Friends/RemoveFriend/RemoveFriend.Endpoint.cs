using FluentValidation;
using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Friends.RemoveFriend;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("friends/{friendId:int}", async (
                int friendId,
                ISender sender,
                IValidator<Command> validator,
                CancellationToken cancellationToken) =>
            {
                var cmd = new Command(friendId);
                await validator.ValidateAndThrowAsync(cmd, cancellationToken);

                await sender.Send(cmd, cancellationToken);
                return Results.NoContent();
            })
            .WithTags("Friends")
            .RequireAuthorization();
    }
}

