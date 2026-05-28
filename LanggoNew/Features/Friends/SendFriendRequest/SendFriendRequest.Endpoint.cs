using FluentValidation;
using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Friends.SendFriendRequest;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("friends/requests", async (
                Command cmd,
                ISender sender,
                IValidator<Command> validator,
                CancellationToken cancellationToken) =>
            {
                await validator.ValidateAndThrowAsync(cmd, cancellationToken);

                await sender.Send(cmd, cancellationToken);
                return TypedResults.Created();
            })
            .WithTags("Friends")
            .RequireAuthorization();
    }
}

