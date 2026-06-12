using Identity.Application.Interfaces;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IOtpService, OtpService>();
            return services;
        }
    }
}
