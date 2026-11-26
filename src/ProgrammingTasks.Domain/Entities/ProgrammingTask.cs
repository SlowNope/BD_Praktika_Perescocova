namespace ProgrammingTasks.Domain.Entities;

public sealed class ProgrammingTask
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Statement { get; set; }
    public required string SolutionIdea { get; set; }
    public string? PolygonUrl { get; set; }
    public bool PreparedForCodeforces { get; set; }
    public bool PreparedForYandexContest { get; set; }
    public int Difficulty { get; set; }
    public string? Note { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<string> Contests { get; set; } = [];
}
