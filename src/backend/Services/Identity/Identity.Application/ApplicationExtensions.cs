using Identity.Application.Interfaces;
using Identity.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
