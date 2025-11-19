using TestCleanArch.Domain.Constants;
using TestCleanArch.Domain.Entities;
using TestCleanArch.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestCleanArch.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
            }
        }

        // Default data
        // Seed, if necessary
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items =
                {
                    new TodoItem { Title = "Make a todo list 📃" },
                    new TodoItem { Title = "Check off the first item ✅" },
                    new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
                    new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
                }
            });

            await _context.SaveChangesAsync();
        }
        if(!_context.Cows.Any())
        {
            var cowNames = new[] { "Bella", "Daisy", "Molly", "Rosie", "Clover", "Bessie", "Buttercup", "Elsie", "Gertie", "Holly", "Iris", "Josie", "Karma", "Lilac", "Maggie", "Nancy", "Olive", "Penny", "Quinn", "Stella" };
            var random = new Random();
            var cows = new List<Cow>();

            for (int i = 0; i < 100; i++)
            {
                var name = cowNames[random.Next(cowNames.Length)];
                var daysAgo = random.Next(365 * 5); // Random date within last 5 years
                var birthDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-daysAgo));

                cows.Add(new Cow { CowName = name, BirthDate = birthDate, CreatedBy="Seeder", Created=DateTime.Now, LastModifiedBy="Seeder", LastModified=DateTime.Now });
            }
            for (int i = 0; i < 70; i++)
            {
                var name = cowNames[random.Next(cowNames.Length)];
                var daysAgo = random.Next(365 * 2) + 365*3; // Random date within last 5 years
                var birthDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-daysAgo));

               cows.Add(new Cow { CowName = name, BirthDate = birthDate, CreatedBy="Seeder", Created=DateTime.Now, LastModifiedBy="Seeder", LastModified=DateTime.Now });
            }

            _context.Cows.AddRange(cows);
            await _context.SaveChangesAsync();
        }

        if(!_context.Chickens.Any())
        {
            var chickenNames = new[] { "Pok", "Hen", "Red", "Dot", "Max", "Tik", "Pep", "Pip", "Top", "Tap", "Rop", "Rok", "Dok", "Jop", "Kik", "Lad", "Muf", "Nor", "Pal", "Pop" };
            var random = new Random();
            var chickens = new List<Chicken>();

            for (int i = 0; i < 100; i++)
            {
                var name = chickenNames[random.Next(chickenNames.Length)];
                var daysAgo = random.Next(365 * 5); // Random date within last 5 years
                var birthDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-daysAgo));

                chickens.Add(new Chicken { ChickenName = name, BirthDate = birthDate, CreatedBy="Seeder", Created=DateTime.Now, LastModifiedBy="Seeder", LastModified=DateTime.Now });
            }
            for (int i = 0; i < 200; i++)
            {
                var name = chickenNames[random.Next(chickenNames.Length)];
                var daysAgo = random.Next(365 * 2); // Random date within last 2 years
                var birthDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-daysAgo));

                chickens.Add(new Chicken { ChickenName = name, BirthDate = birthDate, CreatedBy="Seeder", Created=DateTime.Now, LastModifiedBy="Seeder", LastModified=DateTime.Now });
            }

            _context.Chickens.AddRange(chickens);
            await _context.SaveChangesAsync();
        }
    }
}
