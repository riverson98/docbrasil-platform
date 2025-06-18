using Azure.Identity;
using DocAssociados.ApiGateway.Config;
using DocAssociados.ApiGateway.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var vaultUrl = builder.Configuration["AzureKeyVault:KeyVaultUrl"];
    var uri = $"https://{vaultUrl}.vault.azure.net/";
    
    builder.Configuration.AddAzureKeyVault(
    new Uri(uri),
    new DefaultAzureCredential()
    );

    var apiKeyAuth = builder.Configuration["AssociateAuthKey"];
    var apiKeyAssociados = builder.Configuration["AssociateKey"];

    if (string.IsNullOrEmpty(apiKeyAuth) || string.IsNullOrEmpty(apiKeyAssociados))
        throw new InvalidOperationException("The api keys can't be null or empty");

    builder.Services.AddSingleton(new ApiKeyContainer
    {
        AuthKey = apiKeyAuth,
        AssociadoKey = apiKeyAssociados
    });

    builder.Services.AddTransient<AuthApiKeyHandler>();
    builder.Services.AddTransient<AssociadoApiKeyHandler>();
}
else
{
    var apiKeyAuth = "a0OWFZ7zVSAKuR6ZCyF6pr2UXGAuTdQwdibpalLfPy8=";
    var apiKeyAssociados = "IRBSCgBhWZtwuF5FNzqrIiZR3yabvZTQ5B2PFf8So+c=";

    builder.Services.AddSingleton(new ApiKeyContainer
    {
        AuthKey = apiKeyAuth,
        AssociadoKey = apiKeyAssociados
    });

    builder.Services.AddTransient<AuthApiKeyHandler>();
    builder.Services.AddTransient<AssociadoApiKeyHandler>();
}

// Add services to the container.

builder.Services.AddControllers();

// Configure ocelot
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile($"ocelot.production.development.json", optional: false, reloadOnChange: true);
    builder.Services.AddOcelot()
        .AddDelegatingHandler<AssociadoApiKeyHandler>()
        .AddDelegatingHandler<AuthApiKeyHandler>();
}
else
{
    builder.Configuration.AddJsonFile($"ocelot.production.json", optional: false, reloadOnChange: true);
    builder.Services.AddOcelot()
        .AddDelegatingHandler<AssociadoApiKeyHandler>()
        .AddDelegatingHandler<AuthApiKeyHandler>();
}

//Configure Cors
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("FrontendPolicy",policy =>
        {
           policy.WithOrigins("http://localhost:8080")
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials();
        });
    }
    else
    {
        options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.WithOrigins("http://4.201.160.122:8080")
                      .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    }
});

//Configure auth

var secretKey = builder.Configuration["JWT:Key"]
                    ?? throw new ArgumentException("Invalid secret key");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
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

var app = builder.Build();

app.UseCors("FrontendPolicy");

app.UseRouting();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();

await app.UseOcelot();

await app.RunAsync();
