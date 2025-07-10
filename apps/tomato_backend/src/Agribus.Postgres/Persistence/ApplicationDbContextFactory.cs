namespace Agribus.Postgres.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AgribusDbContext>
{
    public AgribusDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres");

        if (string.IsNullOrEmpty(connectionString))
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            var configuration = new ConfigurationBuilder()
                .SetBasePath(directoryInfo.FullName)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            connectionString = configuration.GetConnectionString("Postgres");
        }

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Could not find connection string");

        var builder = new DbContextOptionsBuilder<AgribusDbContext>();
        builder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new AgribusDbContext(builder.Options);
    }
}
