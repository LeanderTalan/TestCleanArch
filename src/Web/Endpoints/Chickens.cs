using TestCleanArch.Application.Chickens.Commands;
using TestCleanArch.Application.Chickens.Queries.GetChickens;
using Microsoft.AspNetCore.Http.HttpResults;
using TestCleanArch.Application.Chickens.Commands.CreateChicken;
using TestCleanArch.Application.Chickens.Queries.GetChickensPerQuarter;

namespace TestCleanArch.Web.Endpoints;

public class Chickens : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder
            .MapGet(GetChickens);

        groupBuilder
            .MapPost(CreateChicken);

        groupBuilder
            .MapGet(GetChickensPerQuarter, "/per-quarter");
    }

    public async Task<Ok<List<Domain.Entities.Chicken>>> GetChickens(ISender sender)
    {
        var result =  await sender.Send(new GetChickensQuery());
        return TypedResults.Ok(result);
    }

    public async Task<Created<int>> CreateChicken(ISender sender, CreateChickenCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Chickens)}/{id}", id);
    }

    public async Task<Ok<List<QuarterCountDto>>> GetChickensPerQuarter(ISender sender, [AsParameters] GetChickensPerQuarterQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
}