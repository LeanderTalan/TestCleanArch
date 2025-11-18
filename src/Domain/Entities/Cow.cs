using System.ComponentModel.DataAnnotations;

namespace TestCleanArch.Domain.Entities;

public class Cow : BaseAuditableEntity
{
    [MinLength(2), MaxLength(10)]
    public required string CowName { get; set; }
    
    public required DateOnly BirthDate { get; set; }
}