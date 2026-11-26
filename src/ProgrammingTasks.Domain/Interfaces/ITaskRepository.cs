using ProgrammingTasks.Domain.Entities;

namespace ProgrammingTasks.Domain.Interfaces;

public interface ITaskRepository
{
    IReadOnlyList<ProgrammingTask> GetAll();
    void Save(ProgrammingTask task);
    void Delete(int id);
}
