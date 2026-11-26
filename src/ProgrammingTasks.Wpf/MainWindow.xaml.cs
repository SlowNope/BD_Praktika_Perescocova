using System.IO;
using System.Windows;
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

    private void ResetClick(object sender, RoutedEventArgs e) { MinDifficulty.Text = "1"; MaxDifficulty.Text = "10"; TagsFilter.Clear(); Refresh(); }
    private void AddClick(object sender, RoutedEventArgs e) { var editor = new TaskEditorWindow(); if (editor.ShowDialog() == true) { _service.Save(editor.Task); Refresh(); } }
    private void EditClick(object sender, RoutedEventArgs e) { if (TasksGrid.SelectedItem is not TaskRow row) return; var editor = new TaskEditorWindow(row.Task); if (editor.ShowDialog() == true) { _service.Save(editor.Task); Refresh(); } }
    private void DeleteClick(object sender, RoutedEventArgs e) { if (TasksGrid.SelectedItem is not TaskRow row) return; if (MessageBox.Show("Удалить выбранную задачу?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes) { _service.Delete(row.Task.Id); Refresh(); } }

    private sealed class TaskRow
    {
        public TaskRow(ProgrammingTask task) => Task = task;
        public ProgrammingTask Task { get; }
        public string Title => task.Title; public int Difficulty => task.Difficulty;
        public string TagsText => string.Join(", ", task.Tags); public string ContestsText => string.Join(", ", task.Contests);
        public bool PreparedForCodeforces => task.PreparedForCodeforces; public bool PreparedForYandexContest => task.PreparedForYandexContest;
    }
}
