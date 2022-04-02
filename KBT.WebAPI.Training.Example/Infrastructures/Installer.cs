using System;
using KBT.WebAPI.Training.Example.Services;
using KBT.WebAPI.Training.Example.Services.Interfaces;

namespace KBT.WebAPI.Training.Example.Infrastructures
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
