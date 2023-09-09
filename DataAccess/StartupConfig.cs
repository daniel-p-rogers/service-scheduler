using DataAccess.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class StartupConfig
{
    public static void AddDataAccessServices(this IServiceCollection services)
    {
        services.AddSingleton<IDataLoader<PersonModel>, PersonLoaderService>();
        services.AddSingleton<IDataLoader<ServiceRoleModel>, SessionLoaderService>();
        services.AddSingleton<IDataLoader<ServiceDateRoleModel>, ServiceDateRoleLoaderService>();
        services.AddSingleton<IDataLoader<PersonUnavailabilityModel>, PersonUnavailabilityLoaderService>();
    }    
}