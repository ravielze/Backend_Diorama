using Diorama.Internals.Persistent;
using Diorama.RestAPI;
using Diorama.RestAPI.Services;
using Diorama.RestAPI.Repositories;
using Diorama.RestAPI.Controllers;
using Diorama.RestAPI.Middleware;
using Diorama.Internals.Resource;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;
IWebHostEnvironment env = builder.Environment;
IServiceCollection serviceCollections = builder.Services;

serviceCollections.AddDbContext<Database>(opts =>
{
    opts.UseNpgsql(
        config
        .GetConnectionString("Database"))
        .UseSnakeCaseNamingConvention()
        .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
        .EnableSensitiveDataLogging();
});

// Add repository, service & controller layer to the container.
ILayer[] layers = new ILayer[] {
    new Resources(serviceCollections),
    new Repositories(serviceCollections),
    new Services(serviceCollections),
    new Controllers(serviceCollections)
};
foreach (ILayer layer in layers)
{
    layer.Build();
}

serviceCollections.AddEndpointsApiExplorer();
serviceCollections.AddSwaggerGen();
serviceCollections.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();
app.UseMiddleware<ResponseHandlerMiddleware>();
app.UseMiddleware<AuthHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
        {
            opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Diorama v1");
            opts.RoutePrefix = "docs";
        }
    );
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
