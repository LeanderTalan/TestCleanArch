using TestCleanArch.Application.Chickens.Commands.CreateChickenCommand;
using TestCleanArch.Application.Chickens.Queries.GetChickensPerQuarterQuery;

namespace TestCleanArch.Application.FunctionalTests.Chickens.Queries;

using static Testing;

public class GetChickensPerQuarterQueryTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnChickensGroupedByQuarter()
    {
        // Q1 2022
        await SendAsync(new CreateChickenCommand { ChickenName = "Tom", BirthDate = new DateOnly(2022, 1, 15) });
        await SendAsync(new CreateChickenCommand { ChickenName = "Tim", BirthDate = new DateOnly(2022, 2, 20) });

        // Q2 2022
        await SendAsync(new CreateChickenCommand { ChickenName = "Tok", BirthDate = new DateOnly(2022, 4, 10) });

        var query = new GetChickensPerQuarterQuery(null, null);
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task ShouldCalculateCumulativeCount()
    {
        // Q1 2021: 2 chickens
        await SendAsync(new CreateChickenCommand { ChickenName = "Tom", BirthDate = new DateOnly(2021, 1, 15) });
        await SendAsync(new CreateChickenCommand { ChickenName = "Tim", BirthDate = new DateOnly(2021, 3, 20) });

        // Q2 2021: 1 chicken
        await SendAsync(new CreateChickenCommand { ChickenName = "Tok", BirthDate = new DateOnly(2021, 5, 10) });

        var query = new GetChickensPerQuarterQuery(null, null);
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
        await SendAsync(new CreateChickenCommand { ChickenName = "Tom", BirthDate = new DateOnly(2020, 12, 15) });

        // After filter date
        await SendAsync(new CreateChickenCommand { ChickenName = "Tim", BirthDate = new DateOnly(2021, 1, 20) });
        await SendAsync(new CreateChickenCommand { ChickenName = "Tok", BirthDate = new DateOnly(2021, 3, 10) });

        var query = new GetChickensPerQuarterQuery(new DateOnly(2021, 1, 1), null);
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.All(r => r.Quarter.Contains("2021")).ShouldBeTrue();
    }

    [Test]
    public async Task ShouldFilterByDateTo()
    {
        // Within range
        await SendAsync(new CreateChickenCommand { ChickenName = "Tom", BirthDate = new DateOnly(2021, 1, 15) });

        // After filter date
        await SendAsync(new CreateChickenCommand { ChickenName = "Tim", BirthDate = new DateOnly(2022, 6, 20) });

        var query = new GetChickensPerQuarterQuery(null, new DateOnly(2021, 12, 31));
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.All(r => r.Quarter.Contains("2021")).ShouldBeTrue();
    }

    [Test]
    public async Task ShouldReturnEmptyListWhenNoDataInRange()
    {
        var query = new GetChickensPerQuarterQuery(new DateOnly(2030, 1, 1), new DateOnly(2030, 12, 31));
        var result = await SendAsync(query);

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }
}