using ProgrammingTasks.Application;
using ProgrammingTasks.Domain.Entities;
using ProgrammingTasks.Domain.Interfaces;

namespace ProgrammingTasks.Tests;

public sealed class TaskServiceTests
{
    [Fact]
    public void Search_FiltersByDifficultyAndAllSelectedTags()
    {
        var repository = new MemoryRepository(new ProgrammingTask { Id = 1, Title = "A", Statement = "S", SolutionIdea = "I", Difficulty = 5, Tags = ["Графы", "Деревья"] }, new ProgrammingTask { Id = 2, Title = "B", Statement = "S", SolutionIdea = "I", Difficulty = 5, Tags = ["Графы"] });
        var service = new TaskService(repository);

        var result = service.Search(new TaskFilter(4, 6, ["Графы", "Деревья"]));

        Assert.Single(result);
        Assert.Equal("A", result[0].Title);
    }

    [Fact]
    public void Save_AutofillsCodeforcesFlagFromPolygonUrl()
    {
        var repository = new MemoryRepository(); var service = new TaskService(repository);
        var task = new ProgrammingTask { Title = "A", Statement = "S", SolutionIdea = "I", Difficulty = 1, PolygonUrl = "https://polygon.codeforces.com/problem" };

        service.Save(task);

        Assert.True(task.PreparedForCodeforces);
    }

    [Fact]
    public void Save_RejectsDifficultyOutsideRange()
    {
        var service = new TaskService(new MemoryRepository());
        var task = new ProgrammingTask { Title = "A", Statement = "S", SolutionIdea = "I", Difficulty = 11 };

        Assert.Throws<ArgumentOutOfRangeException>(() => service.Save(task));
    }

    private sealed class MemoryRepository(params ProgrammingTask[] tasks) : ITaskRepository
    {
        private readonly List<ProgrammingTask> _tasks = [.. tasks];
        public IReadOnlyList<ProgrammingTask> GetAll() => _tasks;
        public void Save(ProgrammingTask task) => _tasks.Add(task);
        public void Delete(int id) => _tasks.RemoveAll(task => task.Id == id);
    }
}
