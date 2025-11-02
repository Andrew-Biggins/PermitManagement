using PermitManagement.Core.Entities;
using PermitManagement.Presentation.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PermitManagement.Presentation;

public class PermitApiClient(HttpClient http) : IPermitApiClient
{
    private readonly HttpClient _http = http;

    public async Task AddPermitAsync(Permit permit)
    {
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await _http.PostAsJsonAsync("/permits", permit);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<Permit>> GetActivePermitsAsync(string? zone = null, DateTime? date = null)
    {
        var query = string.IsNullOrWhiteSpace(zone)
            ? "/permits/active"
            : $"/permits/active?zone={zone}";

        if (date.HasValue)
            query += $"&date={date.Value:yyyy-MM-dd}";

        return await _http.GetFromJsonAsync<IEnumerable<Permit>>(query) ?? [];
    }

    public async Task<bool> CheckPermitAsync(string reg, string zone)
    {
        var query = $"/permits/check?registration={reg}&zone={zone}";
        return await _http.GetFromJsonAsync<bool>(query);
    }
}