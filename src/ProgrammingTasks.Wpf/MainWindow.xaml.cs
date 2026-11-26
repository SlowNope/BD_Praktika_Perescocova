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
    private int _escapeCount;

    public MainWindow(AuthenticatedUser user)
    {
        ApplicationLogger.Info($"MAIN: конструктор MainWindow, пользователь={user.Login}");
        InitializeComponent();
        var repository = new SqliteTaskRepository(Path.Combine(AppContext.BaseDirectory, "programming-tasks.db"));
        _service = new TaskService(repository);
        UserLabel.Text = $"Пользователь: {user.Login}" + (user.IsAdministrator ? " (администратор)" : " (просмотр)");
        if (!user.IsAdministrator) { AddButton.Visibility = Visibility.Collapsed; EditButton.Visibility = Visibility.Collapsed; DeleteButton.Visibility = Visibility.Collapsed; AdminTab.Visibility = Visibility.Collapsed; }
        Refresh();
        ApplicationLogger.Info("MAIN: данные задач загружены в таблицу");
    }

    private void Refresh(TaskFilter? filter = null) => TasksGrid.ItemsSource = _service.Search(filter ?? TaskFilter.Empty).Select(task => new TaskRow(task)).ToList();

    private void FilterClick(object sender, RoutedEventArgs e)
    {
        var (min, max) = ParseDifficultyRange();
        var tags = TagsFilter.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Refresh(new TaskFilter(min, max, tags));
    }

    private List<ProgrammingTask> CurrentTasks()
    {
        var (min, max) = ParseDifficultyRange();
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

    private (int? Minimum, int? Maximum) ParseDifficultyRange()
    {
        var values = DifficultyRangeText.Text.Replace('–', '-').Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (values.Length != 2 || !int.TryParse(values[0], out var minimum) || !int.TryParse(values[1], out var maximum) || minimum is < 1 or > 10 || maximum is < 1 or > 10 || minimum > maximum)
        { MessageBox.Show("Введите диапазон сложности в формате 1–10.", "Некорректный фильтр", MessageBoxButton.OK, MessageBoxImage.Information); return (null, null); }
        return (minimum, maximum);
    }
    private void ResetClick(object sender, RoutedEventArgs e) { DifficultyRangeText.Text = "1–10"; TagsFilter.Text = ""; Refresh(); }
    private void AddClick(object sender, RoutedEventArgs e) { var editor = new TaskEditorWindow(); if (editor.ShowDialog() == true) { _service.Save(editor.Task); Refresh(); } }
    private void EditClick(object sender, RoutedEventArgs e) { if (TasksGrid.SelectedItem is not TaskRow row) return; var editor = new TaskEditorWindow(row.Task); if (editor.ShowDialog() == true) { _service.Save(editor.Task); Refresh(); } }
    private void DeleteClick(object sender, RoutedEventArgs e) { if (TasksGrid.SelectedItem is not TaskRow row) return; if (MessageBox.Show("Удалить выбранную задачу?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes) { _service.Delete(row.Task.Id); Refresh(); } }
    private void CatalogClick(object sender, RoutedEventArgs e) { var window = new CatalogWindow { Owner = this }; window.ShowDialog(); }
    private void UsersClick(object sender, RoutedEventArgs e) { var window = new UsersWindow { Owner = this }; window.ShowDialog(); }
    private void TaskDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) { if (TasksGrid.SelectedItem is TaskRow row) new TaskDetailsWindow(row.Task) { Owner = this }.ShowDialog(); }
    private void BackClick(object sender, RoutedEventArgs e) { if (MainTabs.SelectedIndex > 0) MainTabs.SelectedIndex--; }
    private void WindowPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key != System.Windows.Input.Key.Escape) { _escapeCount = 0; return; }
        e.Handled = true;
        if (MainTabs.SelectedIndex > 0) { MainTabs.SelectedIndex--; _escapeCount = 0; return; }
        _escapeCount++;
        if (_escapeCount >= 2 && MessageBox.Show("Вы точно хотите закрыть приложение?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) Close();
    }
    private void SwitchUserClick(object sender, RoutedEventArgs e)
    {
        ApplicationLogger.Info("AUTH: пользователь запросил смену пользователя");
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        Close();
        var login = new LoginWindow();
        if (login.ShowDialog() == true) { var window = new MainWindow(login.User!); Application.Current.MainWindow = window; Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose; window.Show(); }
        else Application.Current.Shutdown();
    }

    private sealed class TaskRow
    {
        public TaskRow(ProgrammingTask task) => Task = task;
        public ProgrammingTask Task { get; }
        public string Title => Task.Title; public int Difficulty => Task.Difficulty;
        public string TagsText => string.Join(", ", Task.Tags); public string ContestsText => string.Join(", ", Task.Contests);
        public string CodeforcesStatus => Task.PreparedForCodeforces ? "Готово" : "Не готово";
        public string YandexStatus => Task.PreparedForYandexContest ? "Готово" : "Не готово";
    }
}
