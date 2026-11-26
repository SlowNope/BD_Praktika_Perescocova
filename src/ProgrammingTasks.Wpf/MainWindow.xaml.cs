using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Text;
using ProgrammingTasks.Application;
using ProgrammingTasks.Domain.Entities;
using ProgrammingTasks.Infrastructure;

namespace ProgrammingTasks.Wpf;

public partial class MainWindow : Window
{
    private readonly TaskService _service;

    public MainWindow(AuthenticatedUser user)
    {
        InitializeComponent();
        var repository = new SqliteTaskRepository(Path.Combine(AppContext.BaseDirectory, "programming-tasks.db"));
        _service = new TaskService(repository);
        UserLabel.Text = $"Пользователь: {user.Login}" + (user.IsAdministrator ? " (администратор)" : " (просмотр)");
        if (!user.IsAdministrator) { AddButton.Visibility = Visibility.Collapsed; EditButton.Visibility = Visibility.Collapsed; DeleteButton.Visibility = Visibility.Collapsed; }
        Refresh();
    }

    private void Refresh(TaskFilter? filter = null) => TasksGrid.ItemsSource = _service.Search(filter ?? TaskFilter.Empty).Select(task => new TaskRow(task)).ToList();

    private void FilterClick(object sender, RoutedEventArgs e)
    {
        int.TryParse(MinDifficulty.Text, out var min); int.TryParse(MaxDifficulty.Text, out var max);
        var tags = TagsFilter.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Refresh(new TaskFilter(min, max, tags));
    }

    private List<ProgrammingTask> CurrentTasks()
    {
        int.TryParse(MinDifficulty.Text, out var min); int.TryParse(MaxDifficulty.Text, out var max);
        var tags = TagsFilter.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return _service.Search(new TaskFilter(min, max, tags)).ToList();
    }
    private void ReportClick(object sender, RoutedEventArgs e)
    {
        var tasks = CurrentTasks(); var average = tasks.Count == 0 ? 0 : tasks.Average(task => task.Difficulty);
        MessageBox.Show($"Задач в выборке: {tasks.Count}\nСредняя сложность: {average:F1}\nCodeforces: {tasks.Count(task => task.PreparedForCodeforces)}\nYandex Contest: {tasks.Count(task => task.PreparedForYandexContest)}", "Отчёт");
    }
    private void ExportClick(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "programming-tasks-report.csv" }; if (dialog.ShowDialog() != true) return;
        var csv = new StringBuilder("Название;Сложность;Теги;Контесты;Codeforces;Yandex Contest\n");
        foreach (var task in CurrentTasks()) csv.AppendLine($"{Escape(task.Title)};{task.Difficulty};{Escape(string.Join(", ", task.Tags))};{Escape(string.Join(", ", task.Contests))};{task.PreparedForCodeforces};{task.PreparedForYandexContest}");
        File.WriteAllText(dialog.FileName, csv.ToString(), Encoding.UTF8); MessageBox.Show("Отчёт сохранён.", "Готово");
    }
    private static string Escape(string value) => $"\"{value.Replace("\"", "\"\"")}\"";

    private void ResetClick(object sender, RoutedEventArgs e) { MinDifficulty.Text = "1"; MaxDifficulty.Text = "10"; TagsFilter.Clear(); Refresh(); }
    private void AddClick(object sender, RoutedEventArgs e) { var editor = new TaskEditorWindow(); if (editor.ShowDialog() == true) { _service.Save(editor.Task); Refresh(); } }
    private void EditClick(object sender, RoutedEventArgs e) { if (TasksGrid.SelectedItem is not TaskRow row) return; var editor = new TaskEditorWindow(row.Task); if (editor.ShowDialog() == true) { _service.Save(editor.Task); Refresh(); } }
    private void DeleteClick(object sender, RoutedEventArgs e) { if (TasksGrid.SelectedItem is not TaskRow row) return; if (MessageBox.Show("Удалить выбранную задачу?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes) { _service.Delete(row.Task.Id); Refresh(); } }

    private sealed class TaskRow
    {
        public TaskRow(ProgrammingTask task) => Task = task;
        public ProgrammingTask Task { get; }
        public string Title => Task.Title; public int Difficulty => Task.Difficulty;
        public string TagsText => string.Join(", ", Task.Tags); public string ContestsText => string.Join(", ", Task.Contests);
        public bool PreparedForCodeforces => Task.PreparedForCodeforces; public bool PreparedForYandexContest => Task.PreparedForYandexContest;
    }
}
