using TestCleanArch.Application.Common.Exceptions;
using TestCleanArch.Application.Cows.Commands.CreateCowCommand;
using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.FunctionalTests.Cows.Commands;

using static Testing;

public class CreateCowCommandTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateCowCommand();
        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRequireCowName()
    {
        var command = new CreateCowCommand
        {
            CowName = "",
            BirthDate = new DateOnly(2020, 1, 15)
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRejectCowNameExceedingMaxLength()
    {
        var command = new CreateCowCommand
        {
            CowName = "ThisIsAVeryLongCowName",
            BirthDate = new DateOnly(2020, 1, 15)
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateCow()
    {
        var command = new CreateCowCommand
        {
            CowName = "Bessie",
            BirthDate = new DateOnly(2020, 1, 15)
        };

        var id = await SendAsync(command);

        var cow = await FindAsync<Cow>(id);

        cow.ShouldNotBeNull();
        cow!.CowName.ShouldBe("Bessie");
        cow.BirthDate.ShouldBe(new DateOnly(2020, 1, 15));
    }

    [Test]
    public async Task ShouldCreateMultipleCows()
    {
        var command1 = new CreateCowCommand
        {
            CowName = "Bessie",
            BirthDate = new DateOnly(2020, 1, 15)
        };

        var command2 = new CreateCowCommand
        {
            CowName = "Daisy",
            BirthDate = new DateOnly(2019, 5, 20)
        };

        var id1 = await SendAsync(command1);
        var id2 = await SendAsync(command2);

        var cow1 = await FindAsync<Cow>(id1);
        var cow2 = await FindAsync<Cow>(id2);

        cow1.ShouldNotBeNull();
        cow2.ShouldNotBeNull();
        cow1!.CowName.ShouldBe("Bessie");
        cow2!.CowName.ShouldBe("Daisy");
    }
}