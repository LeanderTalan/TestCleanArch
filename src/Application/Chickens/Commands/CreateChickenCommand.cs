using TestCleanArch.Application.Common.Interfaces;
using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.Chickens.Commands.CreateChickenCommand;

public record CreateChickenCommand : IRequest<int>
{
    //public int ChickenId { get; init; }
    public string? ChickenName { get; init; }
    public DateOnly BirthDate { get; init; }
}

public class CreateChickenCommandHandler : IRequestHandler<CreateChickenCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateChickenCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateChickenCommand request, CancellationToken cancellationToken)
    {
        var entity = new Chicken
        {
            //ChickenId = request.ChickenId,
            ChickenName = request.ChickenName?? "",
            BirthDate = request.BirthDate
        };

        _context.Chickens.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}