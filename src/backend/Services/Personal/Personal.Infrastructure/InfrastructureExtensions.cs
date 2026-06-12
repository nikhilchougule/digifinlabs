using Microsoft.Extensions.DependencyInjection;

namespace Personal.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddPersonalInfrastructure(this IServiceCollection services)
        {
            // Repository implementations are direct GenieDbContext access via PersonalService.
            // Add any external service adapters here (e.g. Account Aggregator client, bureau API).
            return services;
        }
    }
}
