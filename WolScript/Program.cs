using WolScript;
using WolScript.WoL;

const ushort maxTries = 3;

Log.Info("=== Wake-on-LAN Tool ===", true);

// Load or prompt for configuration
Config.Initialise();
Config.Display();

var check = Input.SendConfirm("Do you want to check if PC is online?");
if (!check) return;


var result = TCP.IsOnline();
if (result.Success || result.Refused)
{
    Log.Exit();
    return;
}

check = Input.SendConfirm("Do you want to wake PC on LAN?");
if (!check) return;

if (!Router.SendWakeOnLan())
{
    Log.Exit();
    return;
}

ushort tries = 0;
result = new TcpResult(timedOut: true);
while (result.TimedOut && tries < maxTries)
{
    Log.Info("Waiting a moment for PC to boot...");
    await Task.Delay(10000);

    var res = TCP.IsOnline();
    if (!res.TimedOut)
    {
        result = res;
        break;
    }
    
    tries++;
}

if(tries >= maxTries)
    Log.Error("PC failed to boot after multiple attempts.");

Log.Exit();