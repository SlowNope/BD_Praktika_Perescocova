using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace ProgrammingTasks.Infrastructure;

public sealed record AuthenticatedUser(string Login, bool IsAdministrator);

public sealed class AuthenticationService
{
    private readonly string _connectionString;

    public AuthenticationService(string databasePath)
    {
        _connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath, Pooling = false }.ToString();
        using var connection = new SqliteConnection(_connectionString); connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS Users(Id INTEGER PRIMARY KEY AUTOINCREMENT, Login TEXT UNIQUE NOT NULL, PasswordHash TEXT, Role TEXT NOT NULL); INSERT OR IGNORE INTO Users(Login, PasswordHash, Role) VALUES ('root', $root, 'Administrator'); INSERT OR IGNORE INTO Users(Login, PasswordHash, Role) VALUES ('user1', NULL, 'User');";
        command.Parameters.AddWithValue("$root", Hash("Madmin1!")); command.ExecuteNonQuery();
    }

    public AuthenticatedUser? Authenticate(string login, string password)
    {
        using var connection = new SqliteConnection(_connectionString); connection.Open(); using var command = connection.CreateCommand();
        command.CommandText = "SELECT PasswordHash, Role FROM Users WHERE Login=$login"; command.Parameters.AddWithValue("$login", login);
        using var reader = command.ExecuteReader(); if (!reader.Read()) return null;
        var hash = reader.IsDBNull(0) ? null : reader.GetString(0); var valid = hash is null ? string.IsNullOrEmpty(password) : hash == Hash(password);
        return valid ? new AuthenticatedUser(login, reader.GetString(1) == "Administrator") : null;
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
