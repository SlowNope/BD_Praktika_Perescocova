namespace ProgrammingTasks.Domain.Entities;

/// <summary>Описывает одну задачу по программированию.</summary>
public sealed class ProgrammingTask
{
    // Идентификатор записи в выбранной базе данных.
    public int Id { get; set; }
    /// <summary>Название задачи на русском языке.</summary>
    public required string Title { get; set; }
    /// <summary>Краткое условие задачи.</summary>
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
