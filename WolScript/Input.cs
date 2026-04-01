namespace WolScript;

public static class Input
{
    public const string Confirm = "[Y/N]";
    
    public static string RequestString(string message)
    {
        Log.Plain(message);
        var input = Console.ReadLine();

        Console.WriteLine();
        return input ?? string.Empty;
    }

    public static int? RequestInt(string message)
    {
        Log.Plain(message);
        var input = Console.ReadLine();

        Console.WriteLine();
        return int.TryParse(input, out var result) ? result : null;
    }

    public static bool? RequestBool(string message)
    {
        Log.Plain(message);
        var input = Console.ReadLine();

        Console.WriteLine();
        return CheckBool(input);
    }

    public static bool SendConfirm(string message, bool quitOnNo = true)
    {
        message = message.Trim();

        if (!message.EndsWith(Confirm, StringComparison.OrdinalIgnoreCase))
            message += $" {Confirm}";
        
        Log.Info(message);
        var ans = Console.ReadLine()?.Trim();
        
        Console.WriteLine();
        var result = CheckBool(ans);
        if(!quitOnNo || result) return result;
        
        Log.Plain("Exiting...");
        Environment.Exit(0);
        return false;
    }

    public static string RequestSecureString(string message)
    {
        Log.Plain(message);

        var input = string.Empty;
        ConsoleKeyInfo key;

        while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
                Console.Write("*");
            }
        }

        Console.WriteLine();
        Console.WriteLine();
        return input;
    }

    private static bool CheckBool(string? input)
    {
        if(string.IsNullOrEmpty(input)) 
            return false;
        
        input = input.Trim();
        var success = bool.TryParse(input, out var isBool);
        if(success && isBool) return true;

        return input.Equals("Y", StringComparison.OrdinalIgnoreCase) 
               || input.Equals("YES", StringComparison.OrdinalIgnoreCase);
    }
}