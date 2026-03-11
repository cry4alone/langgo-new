using LanggoNew.Features.Games.JoinGame;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LanggoNew.Features.Games;

public class GameHub(ISender sender) : Hub
{
    public async Task JoinRoom(string roomId, int userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        
        var command = new JoinGame.Command(roomId, userId);
        await sender.Send(command);
        
        await Clients.Group(roomId).SendAsync("ReceiveMessage", $"{userId} has joined the room.");
    }
    
    public async Task LeaveRoom(string roomId, int userId)
    {
        await Clients.Group(roomId).SendAsync("ReceiveMessage", $"{userId} has left the room");
        
        var command = new LeaveGame.Command(userId, roomId);
        await sender.Send(command);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }
    
    public async Task StartGame(string roomName)
    {
        await Clients.Group(roomName).SendAsync("StartGame", $"The game in room {roomName} is starting!");
    }

    public async Task SubmitAnswer(string userName, string roomName, string answer)
    {
        var isCorrect = false;  //go to GameService to check if the answer is correct and update the score
        await Clients.Caller.SendAsync("ReceiveAnswerResult", isCorrect);
    }
    
    public async Task NextRound(string roomName)
    {
        var roundWinner = "Player1";  //go to GameService to determine the winner of the round
        await Clients.Group(roomName).SendAsync("NextRound", $"The next round in room {roomName} is starting!", roundWinner);
    }
    
    public async Task EndGame(string roomName)
    {
        var winner = "Player1";  //go to GameService to determine the winner
        var score = 0; //go to GameService to get the final score
        await Clients.Group(roomName).SendAsync("EndGame", $"The game in room {roomName} has ended!", winner, score);
    }
}