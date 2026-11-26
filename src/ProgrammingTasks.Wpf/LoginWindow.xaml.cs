using System.IO;
using System.Windows;
using ProgrammingTasks.Infrastructure;

namespace ProgrammingTasks.Wpf;

public partial class LoginWindow : Window
{
    public AuthenticatedUser? User { get; private set; }
    public LoginWindow() { InitializeComponent(); }
    private void LoginClick(object sender, RoutedEventArgs e)
    {
        var service = new AuthenticationService(Path.Combine(AppContext.BaseDirectory, "programming-tasks.db"));
        User = service.Authenticate(LoginText.Text.Trim(), PasswordText.Password);
        if (User is null) { MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        DialogResult = true;
    }
}
