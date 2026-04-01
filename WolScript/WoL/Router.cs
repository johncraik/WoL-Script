using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WolScript.WoL;

public static class Router
{
    private const string WolScriptName = "wol";

    /// <summary>
    /// Connects to the router via the API and runs the WoL script.
    /// </summary>
    public static async Task<bool> SendWakeOnLan()
    {
        Log.Info("Sending Wake-on-LAN command...");
        Log.Debug($"Connecting to router at {Config.RouterIp}...");

        try
        {
            Log.Info("Connecting to router REST API...");

            using var client = new HttpClient();

            var auth = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{Config.ApiUser}:{Config.ApiPassword}"));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", auth);

            client.Timeout = TimeSpan.FromSeconds(10);

            var url = $"https://{Config.RouterIp}/rest/system/script/run";

            // Try by script name first
            var payload = new Dictionary<string, string>()
            {
                [".id"] = WolScriptName
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {(int)response.StatusCode}: {body}");

            Log.Success("Wake-on-LAN command sent successfully!", true);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to send WoL command: {ex.Message}", true);
            return false;
        }
    }
}