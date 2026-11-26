using System.Windows;
using ProgrammingTasks.Domain.Entities;

namespace ProgrammingTasks.Wpf;

public partial class TaskDetailsWindow : Window
{
    public TaskDetailsWindow(ProgrammingTask task)
    {
        InitializeComponent(); Title = task.Title; TitleText.Text = task.Title; MetaText.Text = $"Сложность: {task.Difficulty} | Теги: {string.Join(", ", task.Tags)} | Контесты: {string.Join(", ", task.Contests)}"; StatementText.Text = task.Statement; IdeaText.Text = task.SolutionIdea; PolygonText.Text = task.PolygonUrl ?? "не указана"; NoteText.Text = task.Note ?? "нет";
    }
}
