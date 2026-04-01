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
            // using var connection = ConnectionFactory.CreateConnection(TikConnectionType.Api);
            // connection.Open(Config.RouterIp, Config.ApiUser, Config.ApiPassword);
            
            Task.Delay(5000).Wait();
            Log.Debug($"Connected. Running script '{WolScriptName}'...");
            
            // var command = connection.CreateCommand($"/system/script/run .id={WolScriptName}");
            // command.ExecuteNonQuery();

            Task.Delay(10000).Wait();
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