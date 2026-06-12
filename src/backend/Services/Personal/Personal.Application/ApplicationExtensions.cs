using Microsoft.Extensions.DependencyInjection;
using Personal.Application.Interfaces;
using Personal.Application.Services;

namespace Personal.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddPersonalApplication(this IServiceCollection services)
        {
            services.AddScoped<IPersonalService, PersonalService>();
            return services;
        }
    }
}
