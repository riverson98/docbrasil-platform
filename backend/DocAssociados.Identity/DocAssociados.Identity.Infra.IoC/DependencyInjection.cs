using DocAssociados.Identity.Application.Interfaces;
using DocAssociados.Identity.Application.Mappings;
using DocAssociados.Identity.Application.Services;
using DocAssociados.Identity.Domain.Entity;
using DocAssociados.Identity.Domain.Interfaces;
using DocAssociados.Identity.Infra.Data.Context;
using DocAssociados.Identity.Infra.Data.Identity;
using DocAssociados.Identity.Infra.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocAssociados.Identity.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        //Database config
        var mySqlConnection = config.GetConnectionString("MySqlAuthConnection");

        services.AddDbContext<AppDbContext>(options =>
                 options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection),
                 it => it.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        //Identity
        services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        //Config
        services.Configure<Jwt>(config.GetSection("Jwt"));

        //Domain layer
        services.AddScoped<IUserIdentityRepository, UserIdentityRepository>();
        
        //Applicaiton layer
        services.AddScoped<IUserIdentityService, UserIdentityService>();

        //Mapper
        services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

        return services;
    }

    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
