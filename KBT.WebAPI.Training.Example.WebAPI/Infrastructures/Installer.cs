using System;
using KBT.WebAPI.Training.Example.DatabaseFactory.Factory;
using KBT.WebAPI.Training.Example.WebAPI.Services;
using KBT.WebAPI.Training.Example.WebAPI.Services.Interfaces;

namespace KBT.WebAPI.Training.Example.WebAPI.Infrastructures
{
    public class Installer
    {
        private static IServiceCollection _services;

        public static void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;

            // Add Database using Database Factory
            _services.AddScoped<Database>(serviceProvider =>
            {
                string connectionString = configuration.GetConnectionString("DemoDatabase");
                Database database = DBFactory.CreateDatabase("MSSQL", connectionString);
                return database;
            });
            
            // Add UserService
            _services.AddHttpContextAccessor();

            // Add AuthService
            _services.AddTransient<IAuthService, AuthService>();

        }
    }
}
