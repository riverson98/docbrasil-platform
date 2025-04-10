using DocAssociados.Identity.Infra.CrossCutting.Config;
using DocAssociados.Identity.Infra.CrossCutting.KeyVault;
using DocAssociados.Identity.Infra.CrossCutting.Logs;
using DocAssociados.Identity.Infra.CrossCutting.Middles;
using DocAssociados.Identity.Infra.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var authPassword = Environment.GetEnvironmentVariable("AUTH_PASSWORD");
Console.WriteLine($"AUTH_PASSWORD: {authPassword?.Substring(0, 4)}****");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
var secretKey = builder.Configuration["JWT:Key"]
                    ?? throw new ArgumentException("Invalid secret key");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                                                    .GetBytes(secretKey))
    };
});

if (builder.Environment.IsProduction())
{
    var url = new AzureVaultConfig() { KeyVaultUrl = "chavesapisecretas" };
    KeyVaultStatic.Init(url);
    var apiKey = await KeyVaultStatic.GetSecretAsync("ChaveApiAssociadosAuth");

    builder.Services.Configure<ApiSettings>(options =>
    {
        options.ApiKey = apiKey;
    });
}

var app = builder.Build();

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
