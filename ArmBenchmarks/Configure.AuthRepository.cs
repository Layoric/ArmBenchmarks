using ServiceStack;
using ServiceStack.Web;
using ServiceStack.Auth;
using ServiceStack.Configuration;

[assembly: HostingStartup(typeof(ArmBenchmarks.ConfigureAuthRepository))]

namespace ArmBenchmarks
{
    // Custom User Table with extended Metadata properties
    public class AppUser : UserAuth
    {
        public string? ProfileUrl { get; set; }
        public string? LastLoginIp { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    public class ConfigureAuthRepository : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureServices(services => services.AddSingleton<IAuthRepository>(c =>
                new InMemoryAuthRepository<AppUser, UserAuthDetails>()))
            .ConfigureAppHost(appHost => {
                var authRepo = appHost.Resolve<IAuthRepository>();
                authRepo.InitSchema();
                //TODO Delete or change default User's Password 
                CreateUser(authRepo, "admin@email.com", "Admin User", "p@55wOrd", roles: new[]{ AppRoles.Admin });
                CreateUser(authRepo, "manager@email.com", "The Manager", "p@55wOrd", roles: new[] { AppRoles.Employee, AppRoles.Manager });
                CreateUser(authRepo, "employee@email.com", "A Employee", "p@55wOrd", roles: new[] { AppRoles.Employee });
            });

        // Add initial Users to the configured Auth Repository
        public void CreateUser(IAuthRepository authRepo, string email, string name, string password, string[] roles)
        {
            if (authRepo.GetUserAuthByUserName(email) == null)
            {
                var newAdmin = new AppUser { Email = email, DisplayName = name };
                var user = authRepo.CreateUserAuth(newAdmin, password);
                authRepo.AssignRoles(user, roles);
            }
        }
    }
}
