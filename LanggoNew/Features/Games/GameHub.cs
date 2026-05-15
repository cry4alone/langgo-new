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
        
        var command = new JoinGame.Command(roomId, Context.ConnectionId);
        var response = await sender.Send(command);
        
        await Clients.Caller.SendAsync("ReceiveRoomState", response);
    }
    
    public async Task LeaveRoom(string roomId)
    {
        
        var command = new LeaveGame.Command(roomId);
        var response = await sender.Send(command);
        
        await Clients.Group(roomId).SendAsync("ReceiveMessage", $"{response.UserId} has left the room");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }
    
    public async Task StartGame(string roomId)
    {        
        var command = new StartGame.Command(roomId);
        var response = await sender.Send(command);
        
        await Clients.Group(roomId).SendAsync("GameStarted", response.GameStartTime);
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