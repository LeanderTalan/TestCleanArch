using TestCleanArch.Application.Common.Interfaces;

namespace TestCleanArch.Application.Chickens.Queries.GetChickensPerQuarterQuery;

public record GetChickensPerQuarterQueryDto(string Quarter, int Count);

public record GetChickensPerQuarterQuery(DateOnly? DateFrom, DateOnly? DateTo) : IRequest<List<GetChickensPerQuarterQueryDto>>;

public class GetChickensPerQuarterQueryHandler : IRequestHandler<GetChickensPerQuarterQuery, List<GetChickensPerQuarterQueryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetChickensPerQuarterQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetChickensPerQuarterQueryDto>> Handle(GetChickensPerQuarterQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Chickens.AsNoTracking().AsQueryable();

        // Apply date filters if provided
        if (request.DateFrom.HasValue)
        {
            query = query.Where(c => c.BirthDate >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(c => c.BirthDate <= request.DateTo.Value);
        }

        // Group by year and quarter, then count - all in database
        var grouped = await query
            .GroupBy(c => new
            {
                Year = c.BirthDate.Year,
                Quarter = (c.BirthDate.Month - 1) / 3 + 1
            })
            .Select(g => new
            {
                Year = g.Key.Year,
                Quarter = g.Key.Quarter,
                Count = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Quarter)
            .ToListAsync(cancellationToken);

        if (!grouped.Any())
            return new List<GetChickensPerQuarterQueryDto>();

        // Fill in missing quarters
        var minYear = grouped.First().Year;
        var minQuarter = grouped.First().Quarter;
        var maxYear = grouped.Last().Year;
        var maxQuarter = grouped.Last().Quarter;

        var groupedDict = grouped.ToDictionary(g => (g.Year, g.Quarter), g => g.Count);
        var completeData = new List<(int Year, int Quarter, int Count)>();

        for (int year = minYear; year <= maxYear; year++)
        {
            int startQ = (year == minYear) ? minQuarter : 1;
            int endQ = (year == maxYear) ? maxQuarter : 4;

            for (int q = startQ; q <= endQ; q++)
            {
                var count = groupedDict.GetValueOrDefault((year, q), 0);
                completeData.Add((year, q, count));
            }
        }

        // Calculate cumulative counts
        var cumulativeCount = 0;
        var result = completeData.Select(g =>
        {
            cumulativeCount += g.Count;
            return new GetChickensPerQuarterQueryDto($"{g.Year} Q{g.Quarter}", cumulativeCount);
        }).ToList();

        return result;
    }
}