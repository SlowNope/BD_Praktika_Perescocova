namespace ProgrammingTasks.Domain.Entities;

public sealed class Contest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
}
