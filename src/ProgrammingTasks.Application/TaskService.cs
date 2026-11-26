using ProgrammingTasks.Domain.Entities;
using ProgrammingTasks.Domain.Interfaces;
using ProgrammingTasks.Domain.Services;

namespace ProgrammingTasks.Application;

/// <summary>Сервис прикладных операций над задачами.</summary>
public sealed class TaskService(ITaskRepository repository)
{
    /// <summary>Возвращает задачи, соответствующие всем условиям фильтра.</summary>
    public IReadOnlyList<ProgrammingTask> Search(TaskFilter filter)
    {
        return repository.GetAll()
            .Where(task => (!filter.MinimumDifficulty.HasValue || task.Difficulty >= filter.MinimumDifficulty)
                && (!filter.MaximumDifficulty.HasValue || task.Difficulty <= filter.MaximumDifficulty)
                && (filter.Tags.Count == 0 || filter.Tags.All(tag => task.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))))
            .ToList();
    }

    /// <summary>Проверяет задачу и сохраняет её через выбранный репозиторий.</summary>
    public void Save(ProgrammingTask task)
    {
        if (string.IsNullOrWhiteSpace(task.Title)) throw new ArgumentException("Название задачи обязательно.");
        TaskRules.ValidateDifficulty(task.Difficulty);
        task.PreparedForCodeforces = TaskRules.IsPreparedForCodeforces(task.PolygonUrl);
        repository.Save(task);
    }

    public void Delete(int id) => repository.Delete(id);
}
