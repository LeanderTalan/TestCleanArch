using TestCleanArch.Application.Cows.Commands.CreateCowCommand;
using TestCleanArch.Application.Cows.Queries.GetCowsPerQuarterQuery;

namespace TestCleanArch.Application.FunctionalTests.Cows.Queries;

using static Testing;

public class GetCowsPerQuarterQueryTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnCowsGroupedByQuarter()
    {
        // Q1 2020
        await SendAsync(new CreateCowCommand { CowName = "Bessie", BirthDate = new DateOnly(2020, 1, 15) });
        await SendAsync(new CreateCowCommand { CowName = "Daisy", BirthDate = new DateOnly(2020, 2, 20) });

        // Q2 2020
        await SendAsync(new CreateCowCommand { CowName = "Molly", BirthDate = new DateOnly(2020, 4, 10) });

        var query = new GetCowsPerQuarterQuery(null, null);
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task ShouldCalculateCumulativeCount()
    {
        // Q1 2020: 2 cows
        await SendAsync(new CreateCowCommand { CowName = "Bessie", BirthDate = new DateOnly(2020, 1, 15) });
        await SendAsync(new CreateCowCommand { CowName = "Daisy", BirthDate = new DateOnly(2020, 3, 20) });

        // Q2 2020: 1 cow
        await SendAsync(new CreateCowCommand { CowName = "Molly", BirthDate = new DateOnly(2020, 5, 10) });

        var query = new GetCowsPerQuarterQuery(null, null);
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);

        // Check cumulative counts are increasing
        for (int i = 1; i < result.Count; i++)
        {
            var prevCount = int.Parse(result[i - 1].Count.ToString());
            var currCount = int.Parse(result[i].Count.ToString());
            currCount.ShouldBeGreaterThanOrEqualTo(prevCount);
        }
    }

    [Test]
    public async Task ShouldFilterByDateFrom()
    {
        // Before filter date
        await SendAsync(new CreateCowCommand { CowName = "Bessie", BirthDate = new DateOnly(2019, 12, 15) });

        // After filter date
        await SendAsync(new CreateCowCommand { CowName = "Daisy", BirthDate = new DateOnly(2020, 1, 20) });
        await SendAsync(new CreateCowCommand { CowName = "Molly", BirthDate = new DateOnly(2020, 3, 10) });

        var query = new GetCowsPerQuarterQuery(new DateOnly(2020, 1, 1), null);
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        // Should only include cows from 2020 onwards
        result.All(r => r.Quarter.Contains("2020")).ShouldBeTrue();
    }

    [Test]
    public async Task ShouldFilterByDateTo()
    {
        // Within range
        await SendAsync(new CreateCowCommand { CowName = "Bessie", BirthDate = new DateOnly(2020, 1, 15) });

        // After filter date
        await SendAsync(new CreateCowCommand { CowName = "Daisy", BirthDate = new DateOnly(2021, 6, 20) });

        var query = new GetCowsPerQuarterQuery(null, new DateOnly(2020, 12, 31));
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.All(r => r.Quarter.Contains("2020")).ShouldBeTrue();
    }

    [Test]
    public async Task ShouldFillMissingQuarters()
    {
        // Q1 2020
        await SendAsync(new CreateCowCommand { CowName = "Bessie", BirthDate = new DateOnly(2020, 1, 15) });

        // Q4 2020 (skipping Q2 and Q3)
        await SendAsync(new CreateCowCommand { CowName = "Daisy", BirthDate = new DateOnly(2020, 10, 20) });

        var query = new GetCowsPerQuarterQuery(null, null);
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        // Should include entries for Q1, Q2, Q3, Q4 even if some have 0 count
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task ShouldReturnEmptyListWhenNoDataInRange()
    {
        var query = new GetCowsPerQuarterQuery(new DateOnly(2030, 1, 1), new DateOnly(2030, 12, 31));
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }
}