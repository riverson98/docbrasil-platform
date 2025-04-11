using Azure.Identity;
using DocAssociados.ApiGateway.Config;
using DocAssociados.ApiGateway.Handlers;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var vaultUrl = builder.Configuration["AzureKeyVault:KeyVaultUrl"];
    var uri = $"https://{vaultUrl}.vault.azure.net/";
    
    builder.Configuration.AddAzureKeyVault(
    new Uri(uri),
    new DefaultAzureCredential()
    );

    var apiKeyAuth = builder.Configuration["ChaveApiAssociadosAuth"];
    var apiKeyAssociados = builder.Configuration["ChaveApiAssociados"];

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

// Add services to the container.

builder.Services.AddControllers();

//Configure Cors
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("FrontendPolicy",policy =>
        {
           policy.WithOrigins("http://localhost:8080")
                 .AllowAnyMethod()
                 .AllowAnyHeader();
        });
    }
    else
    {
        options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.WithOrigins("http://20.197.248.228:8080")
                      .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    }
});

// Configure ocelot
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

if(builder.Environment.IsDevelopment())
    builder.Configuration.AddJsonFile($"ocelot.production.development.json", optional: false, reloadOnChange: true);
else
    builder.Configuration.AddJsonFile($"ocelot.production.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot()
    .AddDelegatingHandler<AssociadoApiKeyHandler>()
    .AddDelegatingHandler<AuthApiKeyHandler>();
var app = builder.Build();

app.UseCors("FrontendPolicy");

app.UseRouting();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();

await app.UseOcelot();

await app.RunAsync();
