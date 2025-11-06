using Application.Interfaces.Command;
using Application.Interfaces.Queries;
using Infrastructure.Command;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dependencias;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(cfg.GetConnectionString("Default")));

        services.AddScoped<IDoctorAvailabilityQueryRepository, DoctorAvailabilityQueryRepository>();
        services.AddScoped<IDoctorAvailabilityCommandRepository, DoctorAvailabilityCommandRepository>();

        return services;
    }
}
