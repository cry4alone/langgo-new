using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LanggoNew.Features.Games;

public class GameHub(ISender sender) : Hub
{
    public async Task JoinRoom(string roomId, string userName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("ReceiveMessage", $"{userName} has joined the room.");
    }
    
    public async Task LeaveRoom(string roomName, string userName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{userName} has left the room {roomName}.");
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