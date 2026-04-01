namespace WolScript;

public static class Log
{
    private const string SuccessPrefix = "[SUCCESS] ";
    private const string ErrorPrefix = "[ERROR]\t  ";
    private const string InfoPrefix = "[INFO]\t  ";
    private const string WarningPrefix = "[WARN]\t  ";
    private const string DebugPrefix = "[DEBUG]\t  ";
    private const string PlainPrefix = "[LOG]\t  ";
    
    public static void Success(string message, bool lineGap = false)
        => LogMessage(SuccessPrefix, message, lineGap, ConsoleColor.Green);
    
    public static void Error(string message, bool lineGap = false)
        => LogMessage(ErrorPrefix, message, lineGap, ConsoleColor.Red);
    
    public static void Info(string message, bool lineGap = false)
        => LogMessage(InfoPrefix, message, lineGap, ConsoleColor.Cyan);
    
    public static void Warning(string message, bool lineGap = false)
        => LogMessage(WarningPrefix, message, lineGap, ConsoleColor.Yellow);
    
    public static void Debug(string message, bool lineGap = false)
        => LogMessage(DebugPrefix, message, lineGap, ConsoleColor.DarkGray);
    
    public static void Plain(string message, bool lineGap = false)
        => LogMessage(PlainPrefix, message, lineGap, null);

    public static void Exit()
    {
        Plain("Press any key to exit...");
        Console.ReadKey(true);
    }
    
    private static void LogMessage(string prefix, string message, bool lineGap, ConsoleColor? color)
    {
        if(color.HasValue) Console.ForegroundColor = color.Value;
        else Console.ResetColor();
        
        Console.WriteLine($"{prefix}{message}");
        Console.ResetColor();
        
        if (lineGap) Console.WriteLine();
    }
}