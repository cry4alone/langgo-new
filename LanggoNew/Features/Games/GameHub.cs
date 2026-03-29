using LanggoNew.Features.Games.StartNewRound;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LanggoNew.Features.Games;

[Authorize]
public class GameHub(ISender sender) : Hub
{
    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        
        var command = new JoinGame.Command(roomId);
        await sender.Send(command);
        
        await Clients.Group(roomId).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the room.");
    }
    
    public async Task LeaveRoom(string roomId)
    {
        await Clients.Group(roomId).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left the room");
        
        var command = new LeaveGame.Command(roomId);
        await sender.Send(command);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }
    
    public async Task StartGame(string roomId)
    {        
        var command = new StartGame.Command(roomId);
        var response = await sender.Send(command);
        
        await Clients.Group(roomId).SendAsync("GameStarted", response.CurrentWord);
    }

    public async Task SubmitAnswer(string roomId, string answer)
    {
        var command = new CheckAnswer.Command(roomId, answer);
        var result = await sender.Send(command);
        
        if (result is not null)
        {
            await Clients.Group(roomId).SendAsync("ReceiveAnswerResult", result.IsCorrect, result.UserId);
        }
    }
    public async Task FindOpponent(int rating, string langFrom, string langTo)
    {
        await Clients.Caller.SendAsync("OpponentFound", "123");
    }
}