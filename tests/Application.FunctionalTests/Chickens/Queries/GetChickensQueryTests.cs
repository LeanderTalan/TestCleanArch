using TestCleanArch.Application.Chickens.Commands.CreateChickenCommand;
using TestCleanArch.Application.Chickens.Queries.GetChickensQuery;
using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.FunctionalTests.Chickens.Queries;

using static Testing;

public class GetChickensQueryTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnAllChickens()
    {
        await SendAsync(new CreateChickenCommand { ChickenName = "Tim", BirthDate = new DateOnly(2022, 3, 10) });
        await SendAsync(new CreateChickenCommand { ChickenName = "Tom", BirthDate = new DateOnly(2021, 7, 15) });

        var query = new GetChickensQuery();
        var chickens = await SendAsync(query);

        chickens.ShouldNotBeNull();
        chickens.Count.ShouldBeGreaterThanOrEqualTo(2);
        chickens.Any(c => c.ChickenName == "Tim").ShouldBeTrue();
        chickens.Any(c => c.ChickenName == "Tom").ShouldBeTrue();
    }

    [Test]
    public async Task ShouldReturnChickensWithCorrectProperties()
    {
        var id = await SendAsync(new CreateChickenCommand 
        { 
            ChickenName = "Pen", 
            BirthDate = new DateOnly(2020, 6, 12) 
        });

        var query = new GetChickensQuery();
        var chickens = await SendAsync(query);

        var createdChicken = chickens.FirstOrDefault(c => c.Id == id);

        createdChicken.ShouldNotBeNull();
        createdChicken!.ChickenName.ShouldBe("Pen");
        createdChicken.BirthDate.ShouldBe(new DateOnly(2020, 6, 12));
    }

    [Test]
    public async Task ShouldReturnEmptyListWhenNoChickens()
    {
        var query = new GetChickensQuery();
        var chickens = await SendAsync(query);

        chickens.ShouldNotBeNull();
        chickens.ShouldBeOfType<List<Chicken>>();
    }
}