namespace ProgrammingTasks.Domain.Services;

/// <summary>Центральное место для проверок предметной области.</summary>
public static class TaskRules
{
    /// <summary>Проверяет допустимый диапазон сложности 1–10.</summary>
    public static void ValidateDifficulty(int difficulty)
    {
        if (difficulty is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(difficulty), "Сложность должна быть от 1 до 10.");
    }

    /// <summary>Определяет готовность Codeforces по заполненной Polygon-ссылке.</summary>
    public static bool IsPreparedForCodeforces(string? polygonUrl) => !string.IsNullOrWhiteSpace(polygonUrl);
}
