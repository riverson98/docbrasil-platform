using DocAssociados.Infra.IoC;
using DocAssociados.Service.Infra.CrossCutting.AzureIdentity;
using DocAssociados.Service.Infra.CrossCutting.Config;
using DocAssociados.Service.Infra.CrossCutting.Handlers;
using DocAssociados.Service.Infra.CrossCutting.HttpClients.Policys;
using DocAssociados.Service.Infra.CrossCutting.Middles;
using DocAssociados.Service.Infra.IoC;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

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
builder.Services.Configure<ApiGatewayConfig>(builder.Configuration.GetSection("ApiGatewayConfig"));

//Config HttpClient
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<AuthenticatedHttpClientHandler>();

builder.Services.AddHttpClient("DefaultHttpClient")
    .AddPolicyHandler(HttpClientPolicys.GetPolicyWrap())
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();




if (builder.Environment.IsProduction())
{
    var url = new AzureVaultConfig() { KeyVaultUrl = "DocBrasilKeyVault" };
    KeyVaultStatic.Init(url);
    var apiKey = await KeyVaultStatic.GetSecretAsync("AssociateKey");

    builder.Configuration["ApiSecurity:Key"] = apiKey;
}

builder.Services.AddInfrastructure(builder.Configuration);

var secretKey = builder.Configuration["JWT:Key"]
                    ?? throw new ArgumentException("Invalid secret key");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });

//Config Authorization
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminsOnly", policy =>
    {
        policy.RequireRole("Administrador", "Diretor");
    });
});

builder.Logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>(
    "", LogLevel.Information);

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

await app.RunAsync();
