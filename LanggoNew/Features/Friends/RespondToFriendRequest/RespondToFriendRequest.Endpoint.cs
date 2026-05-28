using FluentValidation;
using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Friends.RespondToFriendRequest;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("friends/requests/{requesterId:int}/respond", async (
                int requesterId,
                Request request,
                ISender sender,
                IValidator<Command> validator,
                CancellationToken cancellationToken) =>
            {
                var cmd = new Command(requesterId, request.Accept);
                await validator.ValidateAndThrowAsync(cmd, cancellationToken);

                await sender.Send(cmd, cancellationToken);
                return Results.NoContent();
            })
            .WithTags("Friends")
            .RequireAuthorization();
    }
}

