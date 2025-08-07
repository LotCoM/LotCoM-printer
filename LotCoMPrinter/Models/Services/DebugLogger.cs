namespace LotComPrinter.Models.Services;

public static class DebugLogger
{
    /// <summary>
    /// The debug log directory.
    /// </summary>
    private static readonly string DebugLog = Path.Join(FileSystem.AppDataDirectory, "debug.txt");

    public static void LogMessage(string Message, object Sender)
    {
        // ensure that the debug log exists
        if (!File.Exists(DebugLog))
        {
            File.Create(DebugLog).Close();
        }
        // generate a message log string
        string MessageLog = "";
        MessageLog += $"[Message] {DateTime.Now} | ";
        MessageLog += $"[{Sender.GetType()}]: ";
        MessageLog += Message + "\n";
        // write the Log to the DebugLog file
        File.AppendAllText(DebugLog, MessageLog);
        Console.WriteLine(MessageLog);
    }

    public static void LogWarning(string Message, object Sender)
    {
        // ensure that the debug log exists
        if (!File.Exists(DebugLog))
        {
            File.Create(DebugLog).Close();
        }
        // generate a warning log string
        string WarningLog = "";
        WarningLog += $"[Warning] {DateTime.Now} | ";
        WarningLog += $"[{Sender.GetType()}]: ";
        WarningLog += Message + "\n";
        // write the Log to the DebugLog file
        File.AppendAllText(DebugLog, WarningLog);
        Console.WriteLine(WarningLog);
    }

    public static void LogError(string Message, Exception FaultingException, object Sender)
    {
        // ensure that the debug log exists
        if (!File.Exists(DebugLog))
        {
            File.Create(DebugLog).Close();
        }
        // generate an error log string
        string ErrorLog = "";
        ErrorLog += $"[Error] {DateTime.Now} | ";
        ErrorLog += $"[{Sender.GetType()}]: ";
        ErrorLog += Message + "; ";
        ErrorLog += $"Exception Type: {FaultingException.GetType()}; Exception Message: {FaultingException.Message}.\n";
        // write the Log to the DebugLog file
        File.AppendAllText(DebugLog, ErrorLog);
        Console.WriteLine(ErrorLog);
    }
}