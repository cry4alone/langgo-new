using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using LanggoNew.Shared.Models;
using MediatR;

namespace LanggoNew.Features.Games.CheckAnswer;

public record Command(
    string RoomId,
    string Answer
    ) : IRequest<Response?>;

public record Response(bool IsCorrect, int UserId);

public class Handler(
    IRedisCache cache,
    ICurrentUserService currentUserService,
    ISender sender) : IRequestHandler<Command, Response?>
{
    public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = int.Parse(currentUserService.GetCurrentUserId());
        var shouldEndRound = false;
        
        var response = await cache.ExecuteWithLockAsync(request.RoomId, async gameKey =>
        {
            var currentGameState = await cache.GetDataAsync<GameState>(gameKey);
            if (currentGameState is null)
            {
                throw new NotFoundException($"Game state not found for room '{request.RoomId}'");
            }

            if (currentGameState.IsRoundEnding)
            {
                return new Response(false, currentUserId);
            }

            var isCorrect = string.Equals(
                currentGameState.CurrentWordData.Translation,
                request.Answer,
                StringComparison.CurrentCultureIgnoreCase);

            if (isCorrect)
            {
                currentGameState.UserScores[currentUserId] =
                    currentGameState.UserScores.GetValueOrDefault(currentUserId) + 1;
                currentGameState.RoundWinners.Push(currentUserId);
                currentGameState.IsRoundEnding = true;
                shouldEndRound = true;
            }

            await cache.SetDataAsync(gameKey, currentGameState);
            return new Response(isCorrect, currentUserId);
        });

        if (!shouldEndRound) return response;
        
        await sender.Send(new EndRound.Command(request.RoomId), cancellationToken);
        return null;
    }
}