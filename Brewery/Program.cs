using Brewery.Repositories.Repositories;
using Brewery.Repositories.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Brewery.ServiceCollection;
using Microsoft.AspNetCore.Mvc.Versioning;
using Brewery.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.InjectDependencies();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IOpenBreweryRepository, OpenBreweryRepository>(client =>
{
    client.BaseAddress = new Uri("https://api.openbrewerydb.org/v1/");
    client.DefaultRequestHeaders.Add("User-Agent", "BreweryApi/1.0");
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-api-version")
    );
});

var app = builder.Build();

// Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
