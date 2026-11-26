using Microsoft.Data.Sqlite;

namespace ProgrammingTasks.Infrastructure;

public sealed class CatalogService(string databasePath)
{
    private readonly string _connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();
    public IReadOnlyList<string> GetTags() => GetNames("Tags");
    public IReadOnlyList<string> GetContests() => GetNames("Contests");
    public void AddTag(string name) => AddName("Tags", name);
    public void AddContest(string name) => AddName("Contests", name);
    private IReadOnlyList<string> GetNames(string table) { using var c = Open(); using var command = c.CreateCommand(); command.CommandText = $"SELECT Name FROM {table} ORDER BY Name"; using var reader = command.ExecuteReader(); var result = new List<string>(); while (reader.Read()) result.Add(reader.GetString(0)); return result; }
    private void AddName(string table, string name) { ApplicationLogger.Info($"CATALOG: добавление значения '{name}' в {table}"); if (string.IsNullOrWhiteSpace(name)) return; using var c = Open(); using var command = c.CreateCommand(); command.CommandText = $"INSERT OR IGNORE INTO {table}(Name) VALUES ($name)"; command.Parameters.AddWithValue("$name", name.Trim()); command.ExecuteNonQuery(); }
    private SqliteConnection Open() { var c = new SqliteConnection(_connectionString); c.Open(); return c; }
}
