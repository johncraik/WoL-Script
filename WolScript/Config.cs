namespace WolScript;

public static class Config
{
    private const string EnvPrefix = "WOL_";

    private static readonly ConfigEntry[] Entries =
    [
        new("API_USER", "API Username", false),
        new("API_PASSWORD", "API Password", true),
        new("ROUTER_IP", "Router IP Address", false),
        new("PC_IP", "Target PC IP Address", false),
    ];

    public static string ApiUser => Get("API_USER");
    public static string ApiPassword => Get("API_PASSWORD");
    public static string RouterIp => Get("ROUTER_IP");
    public static string PcIp => Get("PC_IP");

    /// <summary>
    /// Loads config from user environment variables.
    /// Prompts for any missing values and saves them.
    /// </summary>
    public static void Initialise()
    {
        var missing = Entries
            .Where(e => string.IsNullOrWhiteSpace(GetEnvVar(e.Key)))
            .ToList();

        if (missing.Count == 0)
        {
            Log.Success("Configuration loaded from environment variables.");
            return;
        }

        Log.Warning($"{missing.Count} configuration value(s) not set. Please provide them now.");
        Log.Info("These will be saved as user environment variables for future runs.");
        Console.WriteLine();

        foreach (var entry in missing)
        {
            var value = entry.IsSecret
                ? Input.RequestSecureString($"{entry.Prompt}:")
                : Input.RequestString($"{entry.Prompt}:");

            if (string.IsNullOrWhiteSpace(value))
            {
                Log.Error($"{entry.Prompt} cannot be empty. Exiting.");
                Log.Exit();
                Environment.Exit(1);
            }

            Log.Debug($"Setting {entry.Prompt} value...");

            try
            {
                SetEnvVar(entry.Key, value);
                Log.Success($"{entry.Prompt} value set successfully!", true);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to set {entry.Prompt}: {ex.Message}");
                Log.Exit();
                Environment.Exit(1);
            }
        }
    }

    /// <summary>
    /// Displays the current configuration (passwords are masked).
    /// </summary>
    public static void Display()
    {
        Log.Info("Current configuration:");

        foreach (var entry in Entries)
        {
            var value = GetEnvVar(entry.Key);
            var display = string.IsNullOrWhiteSpace(value)
                ? "(not set)"
                : entry.IsSecret
                    ? new string('*', value.Length)
                    : value;

            Log.Plain($"  {entry.Prompt}: {display}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Clears all stored configuration from user environment variables.
    /// </summary>
    public static void Clear()
    {
        foreach (var entry in Entries)
        {
            Log.Debug($"Clearing {entry.Prompt} value from environment variables...");
            SetEnvVar(entry.Key, null!);
        }

        Log.Success("Configuration cleared from environment variables.", true);
    }

    /// <summary>
    /// Re-prompts for all configuration values.
    /// </summary>
    public static void Reconfigure()
    {
        Clear();
        Initialise();
    }

    private static string Get(string key)
    {
        return GetEnvVar(key)
               ?? throw new InvalidOperationException(
                   $"Configuration value '{EnvPrefix}{key}' is not set. Call Config.Initialise() first.");
    }

    private static string? GetEnvVar(string key)
    {
        return Environment.GetEnvironmentVariable($"{EnvPrefix}{key}", EnvironmentVariableTarget.User);
    }

    private static void SetEnvVar(string key, string value)
    {
        Environment.SetEnvironmentVariable($"{EnvPrefix}{key}", value, EnvironmentVariableTarget.User);
        // Also set in current process so it's available immediately
        Environment.SetEnvironmentVariable($"{EnvPrefix}{key}", value, EnvironmentVariableTarget.Process);
    }

    private record ConfigEntry(string Key, string Prompt, bool IsSecret);
}