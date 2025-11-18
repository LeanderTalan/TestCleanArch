using TestCleanArch.Application.Common.Interfaces;
using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.Cows.Commands.CreateCow;

public record CreateCowCommand : IRequest<int>
{
    //public int CowId { get; init; }
    public string? CowName { get; init; }
    public DateOnly BirthDate { get; init; }
}

public class CreateCowCommandHandler : IRequestHandler<CreateCowCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCowCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCowCommand request, CancellationToken cancellationToken)
    {
        var entity = new Cow
        {
            //CowId = request.CowId,
            CowName = request.CowName ?? "",
            BirthDate = request.BirthDate
        };
        
        //TODO: figure out what domain events are for
        //entity.AddDomainEvent(new CowCreatedEvent(entity));
        _context.Cows.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}