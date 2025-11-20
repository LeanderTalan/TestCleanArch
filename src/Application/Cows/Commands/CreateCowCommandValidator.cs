using TestCleanArch.Application.Common.Interfaces;

namespace TestCleanArch.Application.Cows.Commands.CreateCowCommand;

public class CreateCowCommandValidator : AbstractValidator<CreateCowCommand>
{
    private readonly IApplicationDbContext _context;
    
    public CreateCowCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.CowName)
            .NotEmpty().WithMessage("Cow name is required.")
            .MaximumLength(10).WithMessage("Cow name must not exceed 10 characters.");
    }
}