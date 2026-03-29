using LanggoNew.Shared.Enum;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Games.EndGame;

public record Command(string RoomId, GameState CurrentGameState) : IRequest;

public class Handler(IRedisCache cache, AppDbContext dbContext, IHubContext<GameHub> hubContext) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        int? winner = null;

        request.CurrentGameState.Status = GameStatus.Completed;

        var scores = request.CurrentGameState.UserScores;

        if (scores.Count > 0)
        {
            var maxScore = scores.Values.Max();
            var leaders = scores
                .Where(x => x.Value == maxScore)
                .Select(x => x.Key)
                .Take(2)
                .ToList();

            if (leaders.Count == 1)
            {
                winner = leaders[0];
            }
        }

        if (winner.HasValue)
        {
            var winnerExists = await dbContext.Users
                .AnyAsync(u => u.Id == winner.Value, cancellationToken);

            if (!winnerExists)
            {
                winner = null;
            }
        }

        var game = await dbContext.Games
            .Include(g => g.RoundUsers)
            .FirstAsync(g => g.Id == request.CurrentGameState.GameId, cancellationToken);

        game.Status = nameof(GameStatus.Completed);
        game.FinishedAt = DateTime.UtcNow;
        game.WinnerId = winner;


        dbContext.Games.Update(game);
        await dbContext.SaveChangesAsync(cancellationToken);

        await hubContext.Clients.Group(request.RoomId).SendAsync("GameEnded", scores, winner, cancellationToken);
    }
}