using Shouldly;
using TestCleanArch.Application.Cows.Commands.CreateCowCommand;
using TestCleanArch.Application.Cows.Queries.GetCowsQuery;
using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.FunctionalTests.Cows.Queries;

using static Testing;

public class GetCowsQueryTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnAllCows()
    {
        await SendAsync(new CreateCowCommand { CowName = "Bessie", BirthDate = new DateOnly(2020, 1, 15) });
        await SendAsync(new CreateCowCommand { CowName = "Daisy", BirthDate = new DateOnly(2019, 5, 20) });

        var query = new GetCowsQuery();
        var cows = await SendAsync(query);

        cows.ShouldNotBeNull();
        cows.Count.ShouldBeGreaterThanOrEqualTo(2);
        cows.Any(c => c.CowName == "Bessie").ShouldBeTrue();
        cows.Any(c => c.CowName == "Daisy").ShouldBeTrue();
    }

    [Test]
    public async Task ShouldReturnCowsWithCorrectProperties()
    {
        var id = await SendAsync(new CreateCowCommand 
        { 
            CowName = "Molly", 
            BirthDate = new DateOnly(2021, 3, 10) 
        });

        var query = new GetCowsQuery();
        var cows = await SendAsync(query);

        var createdCow = cows.FirstOrDefault(c => c.Id == id);

        createdCow.ShouldNotBeNull();
        createdCow!.CowName.ShouldBe("Molly");
        createdCow.BirthDate.ShouldBe(new DateOnly(2021, 3, 10));
    }

    [Test]
    public async Task ShouldReturnEmptyListWhenNoCows()
    {
        var query = new GetCowsQuery();
        var cows = await SendAsync(query);

        cows.ShouldNotBeNull();
        cows.ShouldBeOfType<List<Cow>>();
    }
}