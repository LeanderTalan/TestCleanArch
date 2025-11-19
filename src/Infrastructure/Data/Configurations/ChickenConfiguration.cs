using TestCleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TestCleanArch.Infrastructure.Data.Configurations;

public class ChickenConfiguration : IEntityTypeConfiguration<Chicken>
{
    public void Configure(EntityTypeBuilder<Chicken> builder)
    {
        builder.HasIndex(c => c.BirthDate);
    }
}
