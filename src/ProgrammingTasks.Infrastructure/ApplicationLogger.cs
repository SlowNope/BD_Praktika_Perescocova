using System.Globalization;

namespace ProgrammingTasks.Infrastructure;

/// <summary>Единый журнал диагностики приложения.</summary>
public static class ApplicationLogger
{
    private static readonly object SyncRoot = new();
    private static readonly string LogDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
    private static readonly string LogFile = Path.Combine(LogDirectory, "application.log");
    public static string LogPath => LogFile;
    public static void Info(string message) => Write("INFO", message);
    public static void Error(string message, Exception? exception = null) => Write("ERROR", exception is null ? message : $"{message} | {exception}");
    private static void Write(string level, string message)
    {
        try { lock (SyncRoot) { Directory.CreateDirectory(LogDirectory); File.AppendAllText(LogFile, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)} [{level}] {message}{Environment.NewLine}"); } }
        catch { }
    }
}
