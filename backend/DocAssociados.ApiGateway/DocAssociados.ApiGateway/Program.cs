using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

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
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName.ToLower()}.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

Debug.WriteLine($"ocelot.{builder.Environment.EnvironmentName.ToLower()}.json");
var app = builder.Build();

app.UseCors("FrontendPolicy");
app.UseRouting();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();

await app.UseOcelot();

await app.RunAsync();
