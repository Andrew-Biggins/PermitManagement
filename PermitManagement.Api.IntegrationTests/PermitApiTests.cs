using Microsoft.AspNetCore.Builder;
using PermitManagement.Core.Entities;
using PermitManagement.Testing.Shared;
using System.Net;
using System.Net.Http.Json;

namespace PermitManagement.Api.IntegrationTests;

public class PermitApiTests
{
    private readonly HttpClient _client;

    public PermitApiTests()
    {
        var builder = WebApplication.CreateBuilder();
        Program.ConfigureServices(builder.Services);
        var app = Program.ConfigureApp(builder);

        app.Urls.Add("http://127.0.0.1:0");
        app.Urls.Add("http://[::1]:0");
        app.StartAsync().GetAwaiter().GetResult();

        _client = new HttpClient { BaseAddress = new Uri(app.Urls.First()) };
    }

    [Gwt("Given a permit",
        "when posted",
        "then successfully created is returned")]
    public async Task T0()
    {
        // Arrange 
        var permit = new
        {
            vehicle = new { registration = "ABC123" },
            zone = new { name = "A" },
            startDate = DateTime.Today,
            endDate = DateTime.Today.AddDays(7)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/permits", permit);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Gwt("Given there are active permits for a zone",
        "when permits for the zone are requested",
        "then the permits are returned")]
    public async Task T1()
    {
        // Arrange
        await _client.PostAsJsonAsync("/permits", new
        {
            vehicle = new { registration = "INT123" },
            zone = new { name = "A" },
            startDate = DateTime.Today.AddDays(-1),
            endDate = DateTime.Today.AddDays(2)
        });

        // Act
        var response = await _client.GetAsync("/permits/active?zone=A");
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<List<Permit>>();

        // Assert
        Assert.NotNull(data);
        Assert.NotEmpty(data);
    }

    [Gwt("Given there are active permits for a zone and vehicle",
        "when permits for the zone and vehicle are requested",
        "then the permits are returned")]
    public async Task T2()
    {
        // Arrange
        await _client.PostAsJsonAsync("/permits", new
        {
            vehicle = new { registration = "CHK123" },
            zone = new { name = "B" },
            startDate = DateTime.Today.AddDays(-1),
            endDate = DateTime.Today.AddDays(2)
        });

        // Act
        var response = await _client.GetAsync("/permits/check?registration=CHK123&zone=B");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<bool>();

        // Assert
        Assert.True(result);
    }
}
