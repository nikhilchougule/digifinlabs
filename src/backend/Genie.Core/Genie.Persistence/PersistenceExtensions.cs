using Genie.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Genie.Persistence
{
    public static class PersistenceExtensions
    {
        /// <summary>
        /// Registers GenieDbContext (Npgsql/PostgreSQL) plus the two EF interceptors.
        /// Call this from every API project's Program.cs:
        ///   builder.Services.AddGeniePersistence(builder.Configuration.GetConnectionString("DefaultConnection")!);
        /// Run migrations from any API project that references Genie.Persistence:
        ///   dotnet ef migrations add InitialCreate --project Genie.Persistence --startup-project Identity.API
        /// </summary>
        public static IServiceCollection AddGeniePersistence(
            this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<AppendOnlyInterceptor>();
            services.AddSingleton<AuditInterceptor>();

            services.AddDbContext<GenieDbContext>((sp, opt) =>
                opt.UseNpgsql(connectionString, npgsql =>
                    {
                        npgsql.MigrationsAssembly(typeof(GenieDbContext).Assembly.FullName);
                        npgsql.EnableRetryOnFailure(maxRetryCount: 3);
                    })
                   .AddInterceptors(
                       sp.GetRequiredService<AppendOnlyInterceptor>(),
                       sp.GetRequiredService<AuditInterceptor>()));

            return services;
        }
    }
}
