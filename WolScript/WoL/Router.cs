using tik4net;

namespace WolScript.WoL;

public static class Router
{
    private const string WolScriptName = "wol";

    /// <summary>
    /// Connects to the router via the API and runs the WoL script.
    /// </summary>
    public static bool SendWakeOnLan()
    {
        Log.Info("Sending Wake-on-LAN command...");
        Log.Debug($"Connecting to router at {Config.RouterIp}...");

        try
        {
            using var connection = ConnectionFactory.CreateConnection(TikConnectionType.Api);
            connection.Open(Config.RouterIp, Config.ApiUser, Config.ApiPassword);
            
            Log.Debug($"Looking up script '{WolScriptName}'...");
            var findScriptCommand = connection.CreateCommand("/system/script/print");

            var scriptRows = findScriptCommand.ExecuteList().ToList();
            if(scriptRows.Count == 0)
                throw new Exception($"Router script '{WolScriptName}' was not found.");
            
            var scriptId = scriptRows.FirstOrDefault(s => s.GetResponseField("name") == WolScriptName)
                ?.GetResponseField(".id")?.ToString();
            
            if (string.IsNullOrWhiteSpace(scriptId))
                throw new Exception($"Router script '{WolScriptName}' did not return a valid .id.");

            Log.Debug($"Running router script '{WolScriptName}' ({scriptId})...");
            
            var runScriptCommand = connection.CreateCommandAndParameters(
                "/system/script/run", TikSpecialProperties.Id, scriptId
            );
            _ = runScriptCommand.ExecuteList().ToList();

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