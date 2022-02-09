using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieProMVC.Data;
using MovieProMVC.Enums;
using MovieProMVC.Models.Database;
using MovieProMVC.Models.Settings;
using Sentry;

namespace MovieProMVC.Services
{
    public class SeedService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ManageDataAsync()
        {
            try
            {
                // Task 1: Create DB from the Migrations
                await UpdateDatabaseAsync();

                // Task 2: Seed a few Roles into the system
                await SeedRolesAsync();

                // Task 3: Seed a few Users into the system
                await SeedUsersAsync();

                // Task 4: Seed default Collection into the system
                await SeedCollectionsAsync();
            }
            catch (Exception err)
            {
                SentrySdk.CaptureException(err);
            }
        }

        private async Task UpdateDatabaseAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }

        private async Task SeedRolesAsync()
        {
            // If there are already Roles in the system: do nothing
            if (_dbContext.Roles.Any()) return;

            // Otherwise we want to create a few Roles
            foreach (var role in Enum.GetNames(typeof(MovieRole)))
            {
                var adminRole = _appSettings.MovieProSettings.DefaultCredentials.Role;
                // Interact with the Role Manager to create Roles
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            // If there are already Users in the system: do nothing
            if (_userManager.Users.Any()) return;

            // Otherwise we want to create a few Users
            // Step 1: Create a new instance of user
            var credentials = _appSettings.MovieProSettings.DefaultCredentials;
            var newUser = new IdentityUser()
            {
                Email = credentials.Email,
                UserName = credentials.Email,
                EmailConfirmed = true
            };

            // Step 2: Interact with the User Manager to create a new User defined by newUser
            // IdentityUser Password needs UpperCase, Number, and Symbol
            var createdUser = await _userManager.CreateAsync(newUser, credentials.Password);

            // Step 3: Add the new User to the Administrator role
            if (await _roleManager.RoleExistsAsync(credentials.Role) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, credentials.Role);
            }

            // Repeat for Demo User
            // Step 1: Create a new instance of user
            var demoUser = new IdentityUser()
            {
                Email = "demo.user@mailinator.com",
                UserName = "demo.user@mailinator.com",
                EmailConfirmed = true
            };

            // Step 2: Interact with the User Manager to create a new User defined by demoUser
            // IdentityUser Password needs UpperCase, Number, and Symbol
            createdUser = await _userManager.CreateAsync(demoUser, "Password123!");

            // Step 3: Add the new User to the User role
            if (await _roleManager.RoleExistsAsync(MovieRole.User.ToString()) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(demoUser, MovieRole.User.ToString());
            }

            // Repeat for Demo Admin
            // Step 1: Create a new instance of user
            var demoAdmin = new IdentityUser()
            {
                Email = "demo.admin@mailinator.com",
                UserName = "demo.admin@mailinator.com",
                EmailConfirmed = true
            };

            // Step 2: Interact with the User Manager to create a new User defined by demoAdmin
            // IdentityUser Password needs UpperCase, Number, and Symbol
            createdUser = await _userManager.CreateAsync(demoAdmin, "Password123!");

            // Step 3: Add the new User to the Administrator role
            if (await _roleManager.RoleExistsAsync(MovieRole.Administrator.ToString()) && createdUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(demoAdmin, MovieRole.Administrator.ToString());
            }
        }

        private async Task SeedCollectionsAsync()
        {
            // If there are already Collections in the system: do nothing
            if (_dbContext.Collection.Any()) return;

            // Otherwise we want to create a new Collection
            // Step 1: Create a new instance of Collection
            _dbContext.Add(new Collection()
            {
                Name = _appSettings.MovieProSettings.DefaultCollection.Name,
                Description = _appSettings.MovieProSettings.DefaultCollection.Description
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
