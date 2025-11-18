using TestCleanArch.Application.Common.Interfaces;
using TestCleanArch.Application.Common.Mappings;
using TestCleanArch.Application.Common.Models;
using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.Chickens.Queries.GetChickens;

public record GetChickensQuery : IRequest<List<Chicken>>;

public class GetChickensQueryHandler : IRequestHandler<GetChickensQuery, List<Chicken>>
{
    private readonly IApplicationDbContext _context;

    public GetChickensQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Chicken>> Handle(GetChickensQuery request, CancellationToken cancellationToken)
    {
        return await _context.Chickens.ToListAsync(cancellationToken);
    }
}