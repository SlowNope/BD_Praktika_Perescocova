using Microsoft.Data.Sqlite;
using ProgrammingTasks.Domain.Entities;
using ProgrammingTasks.Domain.Interfaces;

namespace ProgrammingTasks.Infrastructure;

public sealed class SqliteTaskRepository : ITaskRepository
{
    private readonly string _connectionString;

    public SqliteTaskRepository(string databasePath)
    {
        ApplicationLogger.Info($"DB: инициализация задач, путь={databasePath}");
        _connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();
        Initialize();
    }

    public IReadOnlyList<ProgrammingTask> GetAll()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Title, Statement, SolutionIdea, PolygonUrl, PreparedForCodeforces, PreparedForYandexContest, Difficulty, Note FROM ProgrammingTasks ORDER BY Id DESC";
        using var reader = command.ExecuteReader();
        var result = new List<ProgrammingTask>();
        while (reader.Read())
        {
            var task = new ProgrammingTask
            {
                Id = reader.GetInt32(0), Title = reader.GetString(1), Statement = reader.GetString(2),
                SolutionIdea = reader.GetString(3), PolygonUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                PreparedForCodeforces = reader.GetBoolean(5), PreparedForYandexContest = reader.GetBoolean(6),
                Difficulty = reader.GetInt32(7), Note = reader.IsDBNull(8) ? null : reader.GetString(8)
            };
            LoadLinks(connection, task);
            result.Add(task);
        }
        return result;
    }

    public void Save(ProgrammingTask task)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = task.Id == 0
            ? "INSERT INTO ProgrammingTasks(Title, Statement, SolutionIdea, PolygonUrl, PreparedForCodeforces, PreparedForYandexContest, Difficulty, Note) VALUES ($title,$statement,$idea,$url,$cf,$ya,$difficulty,$note); SELECT last_insert_rowid();"
            : "UPDATE ProgrammingTasks SET Title=$title, Statement=$statement, SolutionIdea=$idea, PolygonUrl=$url, PreparedForCodeforces=$cf, PreparedForYandexContest=$ya, Difficulty=$difficulty, Note=$note WHERE Id=$id";
        AddParameters(command, task);
        if (task.Id == 0) task.Id = Convert.ToInt32(command.ExecuteScalar()); else command.ExecuteNonQuery();
        using var link = connection.CreateCommand();
        link.CommandText = "DELETE FROM TaskTags WHERE TaskId=$id; DELETE FROM TaskContests WHERE TaskId=$id;";
        link.Parameters.AddWithValue("$id", task.Id); link.ExecuteNonQuery();
        InsertLinks(connection, task);
    }

    public void Delete(int id)
    {
        using var connection = Open(); using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM ProgrammingTasks WHERE Id=$id"; command.Parameters.AddWithValue("$id", id); command.ExecuteNonQuery();
    }

    private SqliteConnection Open() { var connection = new SqliteConnection(_connectionString); connection.Open(); return connection; }
    private static void AddParameters(SqliteCommand command, ProgrammingTask task)
    {
        command.Parameters.AddWithValue("$id", task.Id); command.Parameters.AddWithValue("$title", task.Title);
        command.Parameters.AddWithValue("$statement", task.Statement); command.Parameters.AddWithValue("$idea", task.SolutionIdea);
        command.Parameters.AddWithValue("$url", (object?)task.PolygonUrl ?? DBNull.Value); command.Parameters.AddWithValue("$cf", task.PreparedForCodeforces);
        command.Parameters.AddWithValue("$ya", task.PreparedForYandexContest); command.Parameters.AddWithValue("$difficulty", task.Difficulty);
        command.Parameters.AddWithValue("$note", (object?)task.Note ?? DBNull.Value);
    }
    private static void InsertLinks(SqliteConnection connection, ProgrammingTask task)
    {
        foreach (var tag in task.Tags.Distinct(StringComparer.OrdinalIgnoreCase)) InsertLink(connection, "TaskTags", "Tags", tag, task.Id);
        foreach (var contest in task.Contests.Distinct(StringComparer.OrdinalIgnoreCase)) InsertLink(connection, "TaskContests", "Contests", contest, task.Id);
    }
    private static void InsertLink(SqliteConnection connection, string linkTable, string entityTable, string name, int taskId)
    {
        using var command = connection.CreateCommand(); command.CommandText = $"INSERT OR IGNORE INTO {entityTable}(Name) VALUES ($name); INSERT INTO {linkTable}(TaskId, {entityTable[..^1]}Id) SELECT $taskId, Id FROM {entityTable} WHERE Name=$name;";
        command.Parameters.AddWithValue("$name", name); command.Parameters.AddWithValue("$taskId", taskId); command.ExecuteNonQuery();
    }
    private static void LoadLinks(SqliteConnection connection, ProgrammingTask task)
    {
        using var tags = connection.CreateCommand(); tags.CommandText = "SELECT t.Name FROM Tags t JOIN TaskTags l ON l.TagId=t.Id WHERE l.TaskId=$id"; tags.Parameters.AddWithValue("$id", task.Id); using var tr = tags.ExecuteReader(); while (tr.Read()) task.Tags.Add(tr.GetString(0));
        using var contests = connection.CreateCommand(); contests.CommandText = "SELECT c.Name FROM Contests c JOIN TaskContests l ON l.ContestId=c.Id WHERE l.TaskId=$id"; contests.Parameters.AddWithValue("$id", task.Id); using var cr = contests.ExecuteReader(); while (cr.Read()) task.Contests.Add(cr.GetString(0));
    }
    private void Initialize()
    {
        using var connection = Open(); using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys=ON; CREATE TABLE IF NOT EXISTS ProgrammingTasks(Id INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT NOT NULL, Statement TEXT NOT NULL, SolutionIdea TEXT NOT NULL, PolygonUrl TEXT, PreparedForCodeforces INTEGER NOT NULL, PreparedForYandexContest INTEGER NOT NULL, Difficulty INTEGER NOT NULL, Note TEXT); CREATE TABLE IF NOT EXISTS Tags(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE NOT NULL); CREATE TABLE IF NOT EXISTS Contests(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE NOT NULL, Year INTEGER NOT NULL DEFAULT 2024); CREATE TABLE IF NOT EXISTS TaskTags(TaskId INTEGER NOT NULL, TagId INTEGER NOT NULL, PRIMARY KEY(TaskId,TagId), FOREIGN KEY(TaskId) REFERENCES ProgrammingTasks(Id) ON DELETE CASCADE, FOREIGN KEY(TagId) REFERENCES Tags(Id)); CREATE TABLE IF NOT EXISTS TaskContests(TaskId INTEGER NOT NULL, ContestId INTEGER NOT NULL, PRIMARY KEY(TaskId,ContestId), FOREIGN KEY(TaskId) REFERENCES ProgrammingTasks(Id) ON DELETE CASCADE, FOREIGN KEY(ContestId) REFERENCES Contests(Id));";
        command.ExecuteNonQuery();
        using var count = connection.CreateCommand(); count.CommandText = "SELECT COUNT(*) FROM ProgrammingTasks";
        if (Convert.ToInt32(count.ExecuteScalar()) == 0) { ApplicationLogger.Info("DB: демонстрационные задачи отсутствуют, начинается заполнение"); Seed(); ApplicationLogger.Info("DB: демонстрационные задачи добавлены"); }
        else ApplicationLogger.Info("DB: существующие задачи загружены");
    }
    private void Seed()
    {
        Save(new ProgrammingTask { Title="Два указателя", Statement="Найти пару чисел с заданной суммой.", SolutionIdea="Отсортировать массив и двигать два указателя.", PolygonUrl="https://polygon.codeforces.com/", Difficulty=3, PreparedForYandexContest=true, Tags=["Массивы", "Два указателя"], Contests=["Стартовый контест"] });
        Save(new ProgrammingTask { Title="Кратчайший путь", Statement="Найти расстояния от вершины до всех вершин графа.", SolutionIdea="Использовать алгоритм Дейкстры.", Difficulty=7, Tags=["Графы", "Алгоритмы на графах"], Contests=["Алгоритмы 2025"] });
        Save(new ProgrammingTask { Title="Наибольшая общая подпоследовательность", Statement="Вычислить длину LCS двух строк.", SolutionIdea="Динамическое программирование по префиксам строк.", Difficulty=6, PolygonUrl="https://polygon.codeforces.com/", Tags=["Строки", "Динамическое программирование"], Contests=["Алгоритмы 2025"] });
    }
}
