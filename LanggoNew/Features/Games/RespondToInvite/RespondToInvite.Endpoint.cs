using LanggoNew.Endpoints;
using MediatR;

namespace LanggoNew.Features.Games.RespondToInvite;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("games/rooms/{roomId}/invite/respond", async (
                string roomId,
                Request request,
                ISender sender) =>
            {
                var cmd = new Command(roomId, request.Accept);
                await sender.Send(cmd);
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Games");
    }

    public record Request(bool Accept);
}
