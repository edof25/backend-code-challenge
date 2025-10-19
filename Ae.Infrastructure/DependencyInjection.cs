using Ae.Infrastructure.Data;
using Ae.Infrastructure.Interfaces;
using Ae.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Ae.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructures(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IShipRepository, ShipRepository>();
        services.AddScoped<IUserShipRepository, UserShipRepository>();
        services.AddScoped<IFinancialReportRepository, FinancialReportRepository>();

        return services;
    }
}
