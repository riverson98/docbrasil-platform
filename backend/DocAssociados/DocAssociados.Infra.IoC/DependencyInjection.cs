using DocAssociados.Application.Interfaces;
using DocAssociados.Application.Mappings;
using DocAssociados.Application.Services;
using DocAssociados.Domain.Interfaces;
using DocAssociados.Infra.Data.Context;
using DocAssociados.Infra.Data.Repository;
using DocAssociados.Service.Infra.CrossCutting.AzureIdentity;
using DocAssociados.Service.Infra.CrossCutting.Config;
using DocAssociados.Service.Infra.CrossCutting.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DocAssociados.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var mySqlConnection = config.GetConnectionString("MySqlConnection");

        services.AddLogging();

        services.AddDbContext<AppDbContext>(options =>
                 options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection),
                 it => it.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        //Camada de dominio
        services.AddScoped<IAssociadoRepositorio, AssociadoRepositorio>();
        services.AddScoped<IEnderecoRepositorio, EnderecoRepositorio>();
        services.AddScoped<IUnityOfWork, UnityOfWork>();
        services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

        //Camada da aplicacao
        services.AddScoped<IServicoAssociado, ServicoAssociado>();
        services.AddScoped<IServicoEndereco, ServicoEndereco>();
        services.AddScoped(typeof(IServico<,>), typeof(Servico<,>));
        services.AddScoped(typeof(IServicoAzure<>), typeof(ServicoAzure<>));

        //camada do cross cutting
        services.AddScoped<IKeyVaultService, KeyVaultService>();
        services.AddScoped<ILoggerService, LoggerService>();

        //automapper
        services.AddAutoMapper(typeof(DominioParaDtoMappingProfile));

        //configs
        services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<AzureBlobStorageOpcoes>>().Value);
        services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<AzureVaultConfig>>().Value);

        //insights
        services.AddApplicationInsightsTelemetry();

        //cache
        services.AddMemoryCache();

        return services;
    }

}
