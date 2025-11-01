using PermitManagement.Core.Entities;
using System.Net.Http.Json;

namespace PermitManagement.Console;

public class PermitApiClient(HttpClient http)
{
    private readonly HttpClient _http = http;

    public async Task AddPermitAsync(Permit permit)
    {
        var response = await _http.PostAsJsonAsync("/permits", permit);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<Permit>> GetActivePermitsAsync(string zone, DateTime? date = null)
    {
        var query = $"/permits/active?zone={zone}";
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