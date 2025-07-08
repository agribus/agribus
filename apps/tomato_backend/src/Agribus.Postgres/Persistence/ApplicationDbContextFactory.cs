namespace Agribus.Postgres.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AgribusDbContext>
{
    public AgribusDbContext CreateDbContext(string[] args)
    {
        var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        var configuration = new ConfigurationBuilder()
            .SetBasePath(directoryInfo.FullName)
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<AgribusDbContext>();
        builder
            .UseNpgsql(configuration.GetConnectionString("Postgres")!)
            .UseSnakeCaseNamingConvention();

        return new AgribusDbContext(builder.Options);
    }
}
