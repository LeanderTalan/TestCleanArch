using System.ComponentModel.DataAnnotations;

namespace TestCleanArch.Domain.Entities;

public class Chicken : BaseAuditableEntity
{
    [MaxLength(3), MinLength(3)]
    public required string ChickenName { get; set; }
    public required DateOnly BirthDate { get; set; }
}