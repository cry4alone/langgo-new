using LanggoNew.Shared.Exceptions;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.Dictionaries.DeleteDictionary;

public class Handler(
    AppDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<Request>
{
    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
        var dictionary = await context.Dictionaries
            .FirstOrDefaultAsync(d => d.Id == request.DictionaryId, cancellationToken);

        if (dictionary == null)
            throw new NotFoundException("Dictionary not found.");

        var currentUserId = currentUserService.GetCurrentUserId();
        if (dictionary.OwnerId != currentUserId)
            throw new UnauthorizedAccessException();

        dictionary.IsDeleted = true;
        dictionary.DeletedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}
