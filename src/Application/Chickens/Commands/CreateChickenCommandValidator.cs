using TestCleanArch.Application.Common.Interfaces;

namespace TestCleanArch.Application.Chickens.Commands.CreateChickenCommand;

public class CreateChickenCommandValidator : AbstractValidator<CreateChickenCommand>
{
    private readonly IApplicationDbContext _context;
    
    public CreateChickenCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.ChickenName)
            .NotEmpty().WithMessage("Chicken name is required.")
            .MinimumLength(3).WithMessage("Chicken name must be at least 3 characters.")
            .MaximumLength(3).WithMessage("Chicken name must not exceed 3 characters.");
    }
}