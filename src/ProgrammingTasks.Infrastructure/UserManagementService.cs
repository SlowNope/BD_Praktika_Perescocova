using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace ProgrammingTasks.Infrastructure;

public sealed record UserInfo(string Login, string Role);

public sealed class UserManagementService(string databasePath)
{
    private readonly string _connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();
    public IReadOnlyList<UserInfo> GetUsers() { using var c = Open(); using var command = c.CreateCommand(); command.CommandText = "SELECT Login, Role FROM Users ORDER BY Login"; using var reader = command.ExecuteReader(); var result = new List<UserInfo>(); while (reader.Read()) result.Add(new UserInfo(reader.GetString(0), reader.GetString(1))); return result; }
    public void AddManager(string login, string password) { ApplicationLogger.Info($"USERS: добавление менеджера '{login}'"); if (string.IsNullOrWhiteSpace(login) || login.Equals("root", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Укажите корректный логин менеджера."); using var c = Open(); using var command = c.CreateCommand(); command.CommandText = "INSERT INTO Users(Login, PasswordHash, Role) VALUES ($login, $password, 'User')"; command.Parameters.AddWithValue("$login", login.Trim()); command.Parameters.AddWithValue("$password", Hash(password)); command.ExecuteNonQuery(); ApplicationLogger.Info($"USERS: менеджер '{login}' добавлен"); }
    public void Delete(string login) { ApplicationLogger.Info($"USERS: удаление пользователя '{login}'"); if (login.Equals("root", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Учётную запись root нельзя удалить."); using var c = Open(); using var command = c.CreateCommand(); command.CommandText = "DELETE FROM Users WHERE Login=$login"; command.Parameters.AddWithValue("$login", login); command.ExecuteNonQuery(); ApplicationLogger.Info($"USERS: пользователь '{login}' удалён"); }
    private SqliteConnection Open() { var c = new SqliteConnection(_connectionString); c.Open(); return c; }
    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
