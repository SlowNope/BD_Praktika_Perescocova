using Microsoft.Data.Sqlite;
using Npgsql;
using ProgrammingTasks.Domain.Entities;

namespace ProgrammingTasks.Infrastructure;

public sealed class PostgreSqlMigrationService
{
    public async Task TestAndMigrateAsync(PostgreSqlConnectionSettings settings, IReadOnlyList<ProgrammingTask> tasks, CancellationToken cancellationToken = default)
    {
        var builder = new NpgsqlConnectionStringBuilder { Host = settings.Host, Port = settings.Port, Database = settings.Database, Username = settings.Username, Password = settings.Password };
        await using var connection = new NpgsqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        ApplicationLogger.Info($"POSTGRES: подключение успешно к {settings.Host}:{settings.Port}/{settings.Database}");
        await using (var schema = connection.CreateCommand())
        {
            schema.CommandText = "CREATE TABLE IF NOT EXISTS programming_tasks (id SERIAL PRIMARY KEY, title TEXT UNIQUE NOT NULL, statement TEXT NOT NULL, solution_idea TEXT NOT NULL, polygon_url TEXT, prepared_for_codeforces BOOLEAN NOT NULL, prepared_for_yandex BOOLEAN NOT NULL, difficulty INTEGER NOT NULL CHECK (difficulty BETWEEN 1 AND 10), note TEXT); CREATE TABLE IF NOT EXISTS tags (id SERIAL PRIMARY KEY, name TEXT UNIQUE NOT NULL); CREATE TABLE IF NOT EXISTS contests (id SERIAL PRIMARY KEY, name TEXT UNIQUE NOT NULL); CREATE TABLE IF NOT EXISTS task_tags (task_id INTEGER NOT NULL REFERENCES programming_tasks(id) ON DELETE CASCADE, tag_id INTEGER NOT NULL REFERENCES tags(id) ON DELETE CASCADE, PRIMARY KEY(task_id, tag_id)); CREATE TABLE IF NOT EXISTS task_contests (task_id INTEGER NOT NULL REFERENCES programming_tasks(id) ON DELETE CASCADE, contest_id INTEGER NOT NULL REFERENCES contests(id) ON DELETE CASCADE, PRIMARY KEY(task_id, contest_id));";
            await schema.ExecuteNonQueryAsync(cancellationToken);
        }
        foreach (var task in tasks)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO programming_tasks(title, statement, solution_idea, polygon_url, prepared_for_codeforces, prepared_for_yandex, difficulty, note) VALUES(@title,@statement,@idea,@url,@cf,@yandex,@difficulty,@note) ON CONFLICT(title) DO UPDATE SET statement=EXCLUDED.statement, solution_idea=EXCLUDED.solution_idea, polygon_url=EXCLUDED.polygon_url, prepared_for_codeforces=EXCLUDED.prepared_for_codeforces, prepared_for_yandex=EXCLUDED.prepared_for_yandex, difficulty=EXCLUDED.difficulty, note=EXCLUDED.note RETURNING id";
            command.Parameters.AddWithValue("title", task.Title); command.Parameters.AddWithValue("statement", task.Statement); command.Parameters.AddWithValue("idea", task.SolutionIdea); command.Parameters.AddWithValue("url", (object?)task.PolygonUrl ?? DBNull.Value); command.Parameters.AddWithValue("cf", task.PreparedForCodeforces); command.Parameters.AddWithValue("yandex", task.PreparedForYandexContest); command.Parameters.AddWithValue("difficulty", task.Difficulty); command.Parameters.AddWithValue("note", (object?)task.Note ?? DBNull.Value);
            var taskId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
            foreach (var tag in task.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                await using var tagCommand = connection.CreateCommand(); tagCommand.CommandText = "INSERT INTO tags(name) VALUES(@name) ON CONFLICT(name) DO UPDATE SET name=EXCLUDED.name RETURNING id"; tagCommand.Parameters.AddWithValue("name", tag); var tagId = Convert.ToInt32(await tagCommand.ExecuteScalarAsync(cancellationToken));
                await using var linkCommand = connection.CreateCommand(); linkCommand.CommandText = "INSERT INTO task_tags(task_id, tag_id) VALUES(@task,@tag) ON CONFLICT DO NOTHING"; linkCommand.Parameters.AddWithValue("task", taskId); linkCommand.Parameters.AddWithValue("tag", tagId); await linkCommand.ExecuteNonQueryAsync(cancellationToken);
            }
            foreach (var contest in task.Contests.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                await using var contestCommand = connection.CreateCommand(); contestCommand.CommandText = "INSERT INTO contests(name) VALUES(@name) ON CONFLICT(name) DO UPDATE SET name=EXCLUDED.name RETURNING id"; contestCommand.Parameters.AddWithValue("name", contest); var contestId = Convert.ToInt32(await contestCommand.ExecuteScalarAsync(cancellationToken));
                await using var linkCommand = connection.CreateCommand(); linkCommand.CommandText = "INSERT INTO task_contests(task_id, contest_id) VALUES(@task,@contest) ON CONFLICT DO NOTHING"; linkCommand.Parameters.AddWithValue("task", taskId); linkCommand.Parameters.AddWithValue("contest", contestId); await linkCommand.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        ApplicationLogger.Info($"POSTGRES: миграция завершена, обработано задач={tasks.Count}");
    }
}
