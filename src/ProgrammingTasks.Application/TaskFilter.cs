namespace ProgrammingTasks.Application;

public sealed record TaskFilter(int? MinimumDifficulty, int? MaximumDifficulty, IReadOnlyCollection<string> Tags)
{
    public static TaskFilter Empty { get; } = new(null, null, []);
}
