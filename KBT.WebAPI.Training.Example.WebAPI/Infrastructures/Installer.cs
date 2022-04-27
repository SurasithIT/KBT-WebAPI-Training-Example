using System;
using AutoMapper;
using KBT.WebAPI.Training.Example.DatabaseFactory.Factory;
using KBT.WebAPI.Training.Example.WebAPI.Repositories;
using KBT.WebAPI.Training.Example.WebAPI.Repositories.Interfaces;
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
            
            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            
            // Add UserService
            _services.AddHttpContextAccessor();

            // Add AuthService
            _services.AddTransient<IAuthService, AuthService>();

            _services.AddTransient<IUserRepository, UserRepository>();
            _services.AddTransient<IUserService, UserService>();

        }
    }
}
