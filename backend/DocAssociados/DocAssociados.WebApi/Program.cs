using DocAssociados.Infra.IoC;
using DocAssociados.Service.Infra.CrossCutting.AzureIdentity;
using DocAssociados.Service.Infra.CrossCutting.Config;
using DocAssociados.Service.Infra.CrossCutting.Middles;
using DocAssociados.Service.Infra.IoC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

//Configure Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

//CrossCuttingConfig
builder.Services.Configure<AzureBlobStorageOpcoes>(builder.Configuration.GetSection("AzureBlobStorage"));
builder.Services.Configure<AzureVaultConfig>(builder.Configuration.GetSection("AzureKeyVault"));

if (builder.Environment.IsProduction())
{
    var url = new AzureVaultConfig() { KeyVaultUrl = "chavesapisecretas" };
    KeyVaultStatic.Init(url);
    var apiKey = await KeyVaultStatic.GetSecretAsync("ChaveApiAssociados");

    builder.Services.Configure<ApiSettings>(options =>
    {
        options.ApiKey = apiKey;
    });
}

builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

app.ApplyMigrations();

app.UseCors("FrontendPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//middles
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<PerformanceMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
