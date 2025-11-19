using TestCleanArch.Application.Common.Interfaces;

namespace TestCleanArch.Application.Chickens.Queries.GetChickensPerQuarter;

public record QuarterCountDto(string Quarter, int Count);

public record GetChickensPerQuarterQuery(DateOnly? DateFrom, DateOnly? DateTo) : IRequest<List<QuarterCountDto>>;

public class GetChickensPerQuarterQueryHandler : IRequestHandler<GetChickensPerQuarterQuery, List<QuarterCountDto>>
{
    private readonly IApplicationDbContext _context;

    public GetChickensPerQuarterQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuarterCountDto>> Handle(GetChickensPerQuarterQuery request, CancellationToken cancellationToken)
    {
        // Determine date range
        var minDateQuery = _context.Chickens.Select(c => c.BirthDate);
        var minDate = request.DateFrom ?? (await minDateQuery.AnyAsync(cancellationToken) 
            ? await minDateQuery.MinAsync(cancellationToken) 
            : DateOnly.FromDateTime(DateTime.Now));
        
        var maxDate = request.DateTo ?? DateOnly.FromDateTime(DateTime.Now);

        var quarters = GenerateQuarters(minDate, maxDate);

        var result = new List<QuarterCountDto>();

        foreach (var q in quarters)
        {
            var quarterEnd = new DateOnly(q.Year, q.Quarter * 3, DateTime.DaysInMonth(q.Year, q.Quarter * 3));
            
            // Count in database - cumulative count up to quarter end
            var count = await _context.Chickens
                .Where(c => c.BirthDate <= quarterEnd)
                .CountAsync(cancellationToken);
            
            result.Add(new QuarterCountDto($"{q.Year} Q{q.Quarter}", count));
        }

        return result;
    }

    private static List<(int Year, int Quarter)> GenerateQuarters(DateOnly start, DateOnly end)
    {
        var quarters = new List<(int Year, int Quarter)>();
        var startQuarter = (start.Month - 1) / 3 + 1;
        var endQuarter = (end.Month - 1) / 3 + 1;

        for (int year = start.Year; year <= end.Year; year++)
        {
            int firstQ = (year == start.Year) ? startQuarter : 1;
            int lastQ = (year == end.Year) ? endQuarter : 4;

            for (int q = firstQ; q <= lastQ; q++)
            {
                quarters.Add((year, q));
            }
        }

        return quarters;
    }
}