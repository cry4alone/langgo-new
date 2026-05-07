using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanggoNew.Features.User.GetUserProfile;

public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var user = await context.Users 
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);

        return mapper.Map<Response>(user);
    }
}