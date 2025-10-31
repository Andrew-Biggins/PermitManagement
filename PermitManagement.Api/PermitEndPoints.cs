using PermitManagement.Core.Entities;
using PermitManagement.Core.Interfaces;

namespace PermitManagement.Api;

public static class PermitEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/permits", async (Permit permit, IPermitService service) =>
        {
            await service.AddPermitAsync(permit);
            return Results.Created($"/permits/{permit.Id}", permit);
        });

        app.MapGet("/permits/active", async (IPermitService service) =>
            Results.Ok(await service.GetActivePermitsAsync(new Zone("ZoneA"))));

        app.MapGet("/permits/check", async (string registration, string zone, IPermitService service) =>
        {
            var result = await service.HasValidPermitAsync(new Vehicle(registration), new Zone(zone));
            return Results.Ok(result);
        });
    }
}