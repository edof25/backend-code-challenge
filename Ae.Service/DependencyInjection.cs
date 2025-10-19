using Ae.Service.Interfaces;
using Ae.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ae.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IShipService, ShipService>();
        services.AddScoped<IUserShipService, UserShipService>();
        services.AddScoped<IFinancialReportService, FinancialReportService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
