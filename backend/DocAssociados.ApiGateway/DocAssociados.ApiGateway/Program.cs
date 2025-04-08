using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Configure Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy",
        policy =>
        {
            policy.WithOrigins("http://20.197.248.228:8080")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Configure ocelot
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Options)
    {
        context.Response.StatusCode = StatusCodes.Status204NoContent;

        context.Response.Headers["Access-Control-Allow-Origin"] = "http://20.197.248.228:8080";
        context.Response.Headers["Access-Control-Allow-Headers"] = "x-api-key, content-type";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";

        return;
    }

    await next();
});

app.UseCors("FrontendPolicy");

app.UseRouting();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();

await app.UseOcelot();

app.Run();
