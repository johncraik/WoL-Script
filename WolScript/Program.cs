using WolScript;
using WolScript.WoL;

const ushort maxTries = 3;

Log.Info("=== Wake-on-LAN Tool ===", true);

var changeConfig = true;
while (changeConfig)
{
    // Load or prompt for configuration
    Config.Initialise();
    Config.Display();
    
    changeConfig = Input.SendConfirm("Do you want to change configuration?", false);
    if (changeConfig) Config.Reconfigure();
}

// Perform initial check if PC is online
// Exits script if user does not want to check PC (must perform TCP check before WoL)
var check = Input.SendConfirm($"Do you want to check if the PC ({Config.PcIp}) is online?");
if (!check) return;

// Check PC status
var result = await TCP.IsOnline();
if (result.Success || result.Refused)
{
    // Exit when PC is online or refused connection
    Log.Exit();
    return;
}

// Exits script if user does not want to wake PC
check = Input.SendConfirm($"Do you want to wake PC ({Config.PcIp}) on LAN?");
if (!check) return;

if (!Router.SendWakeOnLan())
{
    // Exit if WoL fails
    Log.Exit();
    return;
}

// Retry loop for making TCP connection to PC after WoL command
ushort tries = 0;
result = new TcpResult(timedOut: true);
while (result.TimedOut && tries < maxTries)
{
    var delaySeconds = 20 + (tries * 5);
    Log.Info($"Waiting {delaySeconds} seconds for PC ({Config.PcIp}) to boot...");
    await Task.Delay(delaySeconds * 1000);

    var res = await TCP.IsOnline();
    if (!res.TimedOut)
    {
        // Exit loop if PC is online, refused connection, or error
        result = res;
        break;
    }
    
    tries++;
}

// Log that max retries has been reached
if(tries >= maxTries)
    Log.Error($"PC ({Config.PcIp}) failed to boot after multiple attempts.");

Log.Exit();