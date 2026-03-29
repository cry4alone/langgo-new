using LanggoNew.Models;
using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Dictionaries.AddDictionaryByFile;

public class Handler(AppDbContext context, ICurrentUserService currentUserService) : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentUserId = int.Parse(currentUserService.GetCurrentUserId());
        
        var exists = await context.Dictionaries
            .AnyAsync(d => d.Name == request.Name && d.OwnerId == currentUserId, cancellationToken);

        if (exists)
        {
            throw new ConflictException("Dictionary with the same name already exists");
        }
        
        var dictionary = new Dictionary()
        {
            Name = request.Name,
            LangFrom = request.LangFrom,
            LangTo = request.LangTo,
            Description = request.Description,
            IsPublic = request.IsPublic,
            OwnerId = currentUserId,
            CreatedAt = DateTime.UtcNow,
            Words = request.Entries.Select(e => new DictionaryWord
            {
                Original = e.Original,
                Translation = e.Translation,
                Example = e.Example,
                Difficulty = e.Difficulty
            }).ToList()
        };
        
        context.Dictionaries.Add(dictionary);
        await context.SaveChangesAsync(cancellationToken);
        
        // отправить количество добавленных слов в ответе
    }
}