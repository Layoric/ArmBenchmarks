using Funq;
using ArmBenchmarks.ServiceInterface;
using ServiceStack.DataAnnotations;
using ServiceStack.NativeTypes;

[assembly: HostingStartup(typeof(ArmBenchmarks.AppHost))]

namespace ArmBenchmarks;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            // Configure ASP.NET Core IOC Dependencies
        });

    public AppHost() : base("ArmBenchmarks", typeof(MyServices).Assembly) {}

    public override void Configure(Container container)
    {
        SetConfig(new HostConfig {
        });
        
        Plugins.Add(new CorsFeature(new[] {
            "http://localhost:5173", //vite dev
        }, allowCredentials:true));

    }
}
