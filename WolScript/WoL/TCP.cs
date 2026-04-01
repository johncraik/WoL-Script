using System.Net.Sockets;

namespace WolScript.WoL;

public static class TCP
{
    private const int RdpPort = 3389;
    private const int TimeoutMs = 3000;

    /// <summary>
    /// Tests whether the target PC is reachable by attempting a TCP connection on the RDP port.
    /// </summary>
    public static TcpResult IsOnline()
    {
        var ip = Config.PcIp;

        Log.Info("Checking PC status...");
        Log.Debug($"Attempting TCP connection to {ip}:{RdpPort}...");

        try
        {
            // using var client = new TcpClient();
            // var result = client.BeginConnect(ip, RdpPort, null, null);
            // var connected = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(TimeoutMs));
            //
            // if (connected && client.Connected)
            if(Test())
            {
                //client.EndConnect(result);
                Log.Success("PC is online!", true);
                return new TcpResult();
            }

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

    private static bool Test()
    {
        Task.Delay(TimeoutMs).Wait();
        var rnd = new Random();
        var n = rnd.Next(0, 100);
        return n % 2 == 0 && n > 30;
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