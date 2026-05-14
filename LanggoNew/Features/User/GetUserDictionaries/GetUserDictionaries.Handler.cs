using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.User.GetUserDictionaries;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();
        if (currentUserId != request.UserId) 
            throw new UnauthorizedAccessException();
        
        var wordsCounts = await context.DictionaryWords
            .Join(context.Dictionaries, 
                dw => dw.DictionaryId, 
                d => d.Id, 
                (dw, d) => new { dw, d })
            .Where(x => x.d.OwnerId == request.UserId)
            .GroupBy(x => x.dw.DictionaryId)
            .Select(g => new { DictionaryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DictionaryId, x => x.Count, cancellationToken);
        
        var dictionaries = await context.Dictionaries
            .Where(d => d.IsPublic == true)
            .Where(d => d.OwnerId == request.UserId)
            .Select(d => new DictionaryResponse(
                d.Id, 
                d.Name, 
                d.LangFrom, 
                d.LangTo, 
                d.Description,
                d.Scope,
                wordsCounts.GetValueOrDefault(d.Id, 0)))
            .ToListAsync(cancellationToken);

        return new Response(dictionaries);
    }
}