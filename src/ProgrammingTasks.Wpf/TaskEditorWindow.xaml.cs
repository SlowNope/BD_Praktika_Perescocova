using System.Windows;
using ProgrammingTasks.Domain.Entities;

namespace ProgrammingTasks.Wpf;

public partial class TaskEditorWindow : Window
{
    public ProgrammingTask Task { get; }
    public TaskEditorWindow(ProgrammingTask? task = null)
    {
        KeyDown += (_, args) => { if (args.Key == System.Windows.Input.Key.Escape) { args.Handled = true; Close(); } };
        InitializeComponent(); Task = task is null ? new ProgrammingTask { Title = "", Statement = "", SolutionIdea = "", Difficulty = 1 } : new ProgrammingTask { Id = task.Id, Title = task.Title, Statement = task.Statement, SolutionIdea = task.SolutionIdea, PolygonUrl = task.PolygonUrl, Difficulty = task.Difficulty, Note = task.Note, PreparedForYandexContest = task.PreparedForYandexContest, Tags = [.. task.Tags], Contests = [.. task.Contests] };
        TitleText.Text = Task.Title; StatementText.Text = Task.Statement; IdeaText.Text = Task.SolutionIdea; DifficultyText.Text = Task.Difficulty.ToString(); PolygonText.Text = Task.PolygonUrl; TagsText.Text = string.Join(", ", Task.Tags); ContestsText.Text = string.Join(", ", Task.Contests); NoteText.Text = Task.Note;
    }
    private void SaveClick(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(DifficultyText.Text, out var difficulty) || difficulty is < 1 or > 10 || string.IsNullOrWhiteSpace(TitleText.Text) || string.IsNullOrWhiteSpace(StatementText.Text) || string.IsNullOrWhiteSpace(IdeaText.Text)) { MessageBox.Show("Заполните название, условие, идею решения и укажите сложность от 1 до 10."); return; }
        Task.Title = TitleText.Text.Trim(); Task.Statement = StatementText.Text.Trim(); Task.SolutionIdea = IdeaText.Text.Trim(); Task.Difficulty = difficulty; Task.PolygonUrl = string.IsNullOrWhiteSpace(PolygonText.Text) ? null : PolygonText.Text.Trim(); Task.Tags = TagsText.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(); Task.Contests = ContestsText.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(); Task.Note = NoteText.Text.Trim(); DialogResult = true;
    }
    private void CancelClick(object sender, RoutedEventArgs e) => DialogResult = false;
}
