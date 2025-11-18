
using Microsoft.AspNetCore.Http.HttpResults;
using TestCleanArch.Application.Cows.Commands;
using TestCleanArch.Application.Cows.Commands.CreateCow;
using TestCleanArch.Application.Cows.Queries.GetCows;

namespace TestCleanArch.Web.Endpoints;

public class Cows : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder
            .MapGet(GetCows);

        groupBuilder
            .MapPost(CreateCow);
    }

    public async Task<Ok<List<Domain.Entities.Cow>>> GetCows(ISender sender)
    {
        var result = await sender.Send(new GetCowsQuery());
        return TypedResults.Ok(result);
    }

    public async Task<Created<int>> CreateCow(ISender sender, CreateCowCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Cows)}/{id}", id);
    }
}