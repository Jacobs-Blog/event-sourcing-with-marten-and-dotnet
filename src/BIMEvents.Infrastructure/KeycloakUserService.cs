using System.Net.Http.Json;
using System.Text.Json;

namespace BIMEvents.Infrastructure;

public class KeycloakUserService
{
    private readonly HttpClient _httpClient;
    private readonly string _realm;
    private readonly string _adminUrl;

    public KeycloakUserService(string adminUrl, string realm, string adminUsername, string adminPassword)
    {
        _httpClient = new HttpClient();
        _adminUrl = adminUrl;
        _realm = realm;
        
        var tokenResponse = GetAdminToken(adminUsername, adminPassword).Result;
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse);
    }

    private async Task<string> GetAdminToken(string username, string password)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", "admin-cli"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("grant_type", "password")
        });

        var response = await _httpClient.PostAsync(
            $"{_adminUrl}/realms/master/protocol/openid-connect/token",
            content);

        var result = await response.Content.ReadFromJsonAsync<JsonDocument>();
        return result!.RootElement.GetProperty("access_token").GetString()!;
    }

    public async Task<List<KeycloakUser>> GetUsersAsync()
    {
        var response = await _httpClient.GetAsync($"{_adminUrl}/admin/realms/{_realm}/users");
        return await response.Content.ReadFromJsonAsync<List<KeycloakUser>>() ?? [];
    }
}


public record KeycloakUser(
    string Id,
    string Username,
    string? Email,
    string? FirstName,
    string? LastName
);
