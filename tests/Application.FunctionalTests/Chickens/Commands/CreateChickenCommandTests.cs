using TestCleanArch.Application.Chickens.Commands.CreateChickenCommand;
using TestCleanArch.Application.Common.Exceptions;
using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.FunctionalTests.Chickens.Commands;

using static Testing;

public class CreateChickenCommandTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateChickenCommand();
        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateChicken()
    {
        var command = new CreateChickenCommand
        {
            ChickenName = "Tim",
            BirthDate = new DateOnly(2022, 3, 10)
        };

        var id = await SendAsync(command);

        var chicken = await FindAsync<Chicken>(id);

        chicken.ShouldNotBeNull();
        chicken!.ChickenName.ShouldBe("Tim");
        chicken.BirthDate.ShouldBe(new DateOnly(2022, 3, 10));
    }

    [Test]
    public async Task ShouldCreateMultipleChickens()
    {
        var command1 = new CreateChickenCommand
        {
            ChickenName = "Tim",
            BirthDate = new DateOnly(2022, 3, 10)
        };

        var command2 = new CreateChickenCommand
        {
            ChickenName = "Tom",
            BirthDate = new DateOnly(2021, 7, 15)
        };

        var id1 = await SendAsync(command1);
        var id2 = await SendAsync(command2);

        var chicken1 = await FindAsync<Chicken>(id1);
        var chicken2 = await FindAsync<Chicken>(id2);

        chicken1.ShouldNotBeNull();
        chicken2.ShouldNotBeNull();
        chicken1!.ChickenName.ShouldBe("Tim");
        chicken2!.ChickenName.ShouldBe("Tom");
    }
}