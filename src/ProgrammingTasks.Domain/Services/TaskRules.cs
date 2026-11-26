namespace ProgrammingTasks.Domain.Services;

public static class TaskRules
{
    public static void ValidateDifficulty(int difficulty)
    {
        if (difficulty is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(difficulty), "Сложность должна быть от 1 до 10.");
    }

    public static bool IsPreparedForCodeforces(string? polygonUrl) => !string.IsNullOrWhiteSpace(polygonUrl);
}
