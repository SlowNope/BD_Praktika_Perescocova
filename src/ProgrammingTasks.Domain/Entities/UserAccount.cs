namespace ProgrammingTasks.Domain.Entities;

public sealed class UserAccount
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public string? PasswordHash { get; set; }
    public required string Role { get; set; }
}
