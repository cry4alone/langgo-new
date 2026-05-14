using LanggoNew.Shared.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Dictionaries.GetDictionaries;

public class Handler(AppDbContext context) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var wordsCounts = await context.DictionaryWords
            .GroupBy(dw => dw.DictionaryId)
            .Select(g => new { DictionaryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DictionaryId, x => x.Count, cancellationToken);

        var dictionaries = await context.Dictionaries
            .Where(d => d.IsPublic == true)
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

