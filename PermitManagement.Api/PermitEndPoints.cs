using FluentValidation;
using PermitManagement.Core.Entities;
using PermitManagement.Core.Interfaces;

namespace PermitManagement.Api;

public static class PermitEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/permits", async (Permit permit, IValidator<Permit> validator, IPermitService service) =>
        {
            var result = await validator.ValidateAsync(permit);
            if (!result.IsValid)
                return Results.BadRequest(result.Errors.Select(e => e.ErrorMessage));

            await service.AddPermitAsync(permit);
            return Results.Created($"/permits/{permit.Id}", permit);
        });

        app.MapGet("/permits/active", async (string? zone, DateTime? date, IPermitService service) =>
        {
            var results = string.IsNullOrWhiteSpace(zone)
                ? await service.GetAllActivePermitsAsync(date)
                : await service.GetActivePermitsAsync(new Zone (zone), date);

            return Results.Ok(results);
        })
        .WithDescription("Gets all active permits. Optional 'zone' and 'date' parameters check activity for a specific zone or date.");

        app.MapGet("/permits/check", async (string registration, string zone, IPermitService service) =>
        {
            var result = await service.HasValidPermitAsync(new Vehicle(registration), new Zone(zone));
            return Results.Ok(result);
        });
    }
}