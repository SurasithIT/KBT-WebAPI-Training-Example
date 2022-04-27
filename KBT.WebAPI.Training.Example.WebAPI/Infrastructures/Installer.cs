using System;
using KBT.WebAPI.Training.Example.WebAPI.Services;
using KBT.WebAPI.Training.Example.WebAPI.Services.Interfaces;

namespace KBT.WebAPI.Training.Example.WebAPI.Infrastructures
{
    public class Installer
    {
        private static IServiceCollection _services;

        public static void InstallService(IServiceCollection services)
        {
            _services = services;

            // Add UserService
            _services.AddHttpContextAccessor();

            // Add AuthService
            _services.AddTransient<IAuthService, AuthService>();

        }
    }
}
