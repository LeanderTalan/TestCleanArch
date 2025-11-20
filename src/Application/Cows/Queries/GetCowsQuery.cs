using TestCleanArch.Application.Common.Interfaces;
using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.Cows.Queries.GetCowsQuery;
public record GetCowsQuery: IRequest<List<Cow>>;

public class GetCowsQueryHandler : IRequestHandler<GetCowsQuery, List<Cow>>
{
    private readonly IApplicationDbContext _context;

    public GetCowsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cow>> Handle(GetCowsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Cows.ToListAsync(cancellationToken);
    }
}