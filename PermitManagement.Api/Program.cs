using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PermitManagement.Core.Interfaces;
using PermitManagement.Core.Services;
using PermitManagement.Infrastructure;
using System.Text.Json;

namespace PermitManagement.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);
        var app = ConfigureApp(builder);
        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDbContext<PermitDbContext>(
            o => o.UseSqlite("Data Source=permit.db"));

        services.AddScoped<IPermitRepository, PermitRepository>();
        services.AddScoped<IPermitService, PermitService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddValidatorsFromAssemblyContaining<PermitValidator>();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    public static WebApplication ConfigureApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PermitDbContext>();

#if DEBUG
            context.Database.EnsureDeleted(); // Only in dev
#endif

            context.Database.EnsureCreated();
            SeedData.Initialize(context);
        }

        app.UseHttpsRedirection();
        PermitEndpoints.Map(app);

        return app;
    }
}