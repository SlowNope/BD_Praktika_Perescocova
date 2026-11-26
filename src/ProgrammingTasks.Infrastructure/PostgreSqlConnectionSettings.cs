using System.Text.Json;

namespace ProgrammingTasks.Infrastructure;

public sealed class PostgreSqlConnectionSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = "programming_tasks";
    public string Username { get; set; } = "postgres";
    public string Password { get; set; } = "postgres";
    public bool IsConfigured { get; set; }
}

public sealed class PostgreSqlSettingsStore
{
    private readonly string _path = Path.Combine(AppContext.BaseDirectory, "postgres-connection.json");
    public PostgreSqlConnectionSettings Load()
    {
        try { return File.Exists(_path) ? JsonSerializer.Deserialize<PostgreSqlConnectionSettings>(File.ReadAllText(_path)) ?? new() : new(); }
        catch (Exception exception) { ApplicationLogger.Error("POSTGRES: ошибка чтения настроек", exception); return new(); }
    }
    public void Save(PostgreSqlConnectionSettings settings)
    {
        settings.IsConfigured = true;
        File.WriteAllText(_path, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        ApplicationLogger.Info($"POSTGRES: настройки сохранены, сервер={settings.Host}:{settings.Port}, база={settings.Database}");
    }
}
