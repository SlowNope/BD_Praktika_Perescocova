using System.IO;
using System.Windows;
using ProgrammingTasks.Infrastructure;

namespace ProgrammingTasks.Wpf;

public partial class CatalogWindow : Window
{
    private readonly CatalogService _service = new(Path.Combine(AppContext.BaseDirectory, "programming-tasks.db"));
    public CatalogWindow() { InitializeComponent(); Refresh(); }
    private void Refresh() { TagsList.ItemsSource = _service.GetTags(); ContestsList.ItemsSource = _service.GetContests(); }
    private void AddTagClick(object sender, RoutedEventArgs e) { _service.AddTag(TagText.Text); TagText.Clear(); Refresh(); }
    private void AddContestClick(object sender, RoutedEventArgs e) { _service.AddContest(ContestText.Text); ContestText.Clear(); Refresh(); }
}
