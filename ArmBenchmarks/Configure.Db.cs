using ServiceStack.Data;
using ServiceStack.OrmLite;

[assembly: HostingStartup(typeof(ArmBenchmarks.ConfigureDb))]

namespace ArmBenchmarks;

// Database can be created with "dotnet run --AppTasks=migrate"   
public class ConfigureDb : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context,services) => services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
            context.Configuration.GetConnectionString("DefaultConnection") ?? "App_Data/db.sqlite",
            SqliteDialect.Provider)));
}
