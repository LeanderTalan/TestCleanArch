using TestCleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TestCleanArch.Infrastructure.Data.Configurations;

public class CowConfiguration : IEntityTypeConfiguration<Cow>
{
    public void Configure(EntityTypeBuilder<Cow> builder)
    {
        // builder.Property(t => t.CowName)
        //     .IsRequired();
    }
}
