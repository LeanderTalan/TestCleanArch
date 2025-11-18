using TestCleanArch.Domain.Entities;

namespace TestCleanArch.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    DbSet<Cow> Cows { get; }
    DbSet<Chicken> Chickens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
