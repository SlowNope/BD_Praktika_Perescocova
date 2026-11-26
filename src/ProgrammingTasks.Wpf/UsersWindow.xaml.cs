using System.IO;
using System.Windows;
using ProgrammingTasks.Infrastructure;

namespace ProgrammingTasks.Wpf;

public partial class UsersWindow : Window
{
    private readonly UserManagementService _service = new(Path.Combine(AppContext.BaseDirectory, "programming-tasks.db"));
    public UsersWindow() { KeyDown += (_, args) => { if (args.Key == System.Windows.Input.Key.Escape) { args.Handled = true; Close(); } }; InitializeComponent(); Refresh(); }
    private void Refresh() => UsersList.ItemsSource = _service.GetUsers();
    private void AddClick(object sender, RoutedEventArgs e) { try { _service.AddManager(LoginText.Text, PasswordText.Password); LoginText.Clear(); PasswordText.Clear(); Refresh(); } catch (Exception exception) { MessageBox.Show(exception.Message, "Ошибка"); } }
    private void DeleteClick(object sender, RoutedEventArgs e) { if (UsersList.SelectedItem is not UserInfo user) return; if (MessageBox.Show($"Удалить пользователя {user.Login}?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return; try { _service.Delete(user.Login); Refresh(); } catch (Exception exception) { MessageBox.Show(exception.Message, "Ошибка"); } }
}
