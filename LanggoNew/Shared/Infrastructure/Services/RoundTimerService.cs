using Hangfire;
using LanggoNew.Shared.Config;
using MediatR;
using Microsoft.Extensions.Options;
using LanggoNew.Features.Games.EndRound;
using LanggoNew.Features.Games.StartNewRound;

namespace LanggoNew.Shared.Infrastructure.Services;


public interface IGameTimerService
{
    Task<string> ScheduleEndRound(string roomId);
    Task<string> ScheduleStartNewRound(string roomId);
    Task CancelJob(string? jobId);
}

public class GameTimerService(IBackgroundJobClient backgroundJobClient, IOptions<GameTimingOptions> timingOptions) : IGameTimerService
{
    public Task<string> ScheduleEndRound(string roomId)
    {
        var delay = TimeSpan.FromSeconds(timingOptions.Value.RoundDurationSeconds); 
        var jobId = backgroundJobClient.Schedule<EndRoundByTimerJob>(
            job => job.Execute(roomId)
            , delay);
        
        return Task.FromResult(jobId);
    }

    public Task<string> ScheduleStartNewRound(string roomId)
    {
        var delay = TimeSpan.FromSeconds(timingOptions.Value.PauseBetweenRoundsSeconds); 
        var jobId = backgroundJobClient.Schedule<StartNewRoundByTimerJob>(
            job => job.Execute(roomId)
            , delay);

        return Task.FromResult(jobId);
    }

    public Task CancelJob(string? jobId)
    {
        backgroundJobClient.Delete(jobId);
        
        return Task.CompletedTask;
    }
}

public class EndRoundByTimerJob(ISender sender)
{
    public async Task Execute(string roomId)
    {
        await sender.Send(new Features.Games.EndRound.Command(roomId));
    }
}

public class StartNewRoundByTimerJob(ISender sender)
{
    public async Task Execute(string roomId)
    {
        await sender.Send(new Features.Games.StartNewRound.Command(roomId));
    }
}