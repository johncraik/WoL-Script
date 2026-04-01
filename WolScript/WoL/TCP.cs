using System.Net.Sockets;

namespace WolScript.WoL;

public static class TCP
{
    private const int RdpPort = 3389;
    private const int TimeoutMs = 5000;

    /// <summary>
    /// Tests whether the target PC is reachable by attempting a TCP connection on the RDP port.
    /// </summary>
    public static async Task<TcpResult> IsOnline()
    {
        var ip = Config.PcIp;

        Log.Info("Checking PC status...");
        Log.Debug($"Attempting TCP connection to {ip}:{RdpPort}...");

        try
        {
            using var client = new TcpClient();
            using var cts = new CancellationTokenSource(TimeoutMs);
            await client.ConnectAsync(ip, RdpPort, cts.Token);

            Log.Success("PC is online!", true);
            return new TcpResult();
        }
        catch (OperationCanceledException)
        {
            Log.Warning("PC is offline (connection timed out).", true);
            return new TcpResult(timedOut: true);
        }
        catch (SocketException)
        {
            Log.Error("PC is offline (connection refused).", true);
            return new TcpResult(refused: true);
        }
        catch (Exception ex)
        {
            Log.Error($"Connection check failed: {ex.Message}", true);
            return new TcpResult(error: true);
        }
    }
}

public class TcpResult
{
    public bool Success => !TimedOut && !Refused && !Error;
    public bool TimedOut { get; }
    public bool Refused { get; }
    public bool Error { get; }

    public TcpResult()
    {
    }

    public TcpResult(bool timedOut = false, bool refused = false, bool error = false)
    {
        TimedOut = timedOut;
        Refused = refused;
        Error = error;
    }
}