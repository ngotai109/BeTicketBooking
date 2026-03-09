using System.IO;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BookingTicket.Infrastructure.Data
{
    /// <summary>
    /// Design-time factory for EF Core tools (dotnet ef).
    /// This avoids requiring the API host/DI container to be constructed.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // dotnet-ef may execute with various working directories; locate repo root by walking up
            var basePath = FindRepoRoot(Directory.GetCurrentDirectory());

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(Path.Combine("BookingTicket.API", "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine("BookingTicket.API", "appsettings.Development.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found. " +
                    "Please set it in BookingTicket.API/appsettings.Development.json (ConnectionStrings:DefaultConnection)."
                );
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private static string FindRepoRoot(string startDirectory)
        {
            var dir = new DirectoryInfo(startDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "BookingTicket.API", "appsettings.Development.json");
                if (File.Exists(candidate))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }

            // Fallback: current directory (may still work if env vars provide connection string)
            return startDirectory;
        }
    }
}

