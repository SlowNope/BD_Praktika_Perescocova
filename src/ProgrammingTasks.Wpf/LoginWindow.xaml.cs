using System.IO;
using System.Windows;
using ProgrammingTasks.Infrastructure;

namespace ProgrammingTasks.Wpf;

public partial class LoginWindow : Window
{
    public AuthenticatedUser? User { get; private set; }
    public LoginWindow() { KeyDown += (_, args) => { if (args.Key == System.Windows.Input.Key.Escape) { args.Handled = true; Close(); } }; ApplicationLogger.Info("AUTH: конструктор LoginWindow"); InitializeComponent(); }
    private void LoginClick(object sender, RoutedEventArgs e)
    {
        ApplicationLogger.Info($"AUTH: нажата кнопка входа, логин='{LoginText.Text.Trim()}'");
        try
        {
            var service = new AuthenticationService(Path.Combine(AppContext.BaseDirectory, "programming-tasks.db"));
            User = service.Authenticate(LoginText.Text.Trim(), PasswordText.Password);
        }
        catch (Exception exception)
        {
            ApplicationLogger.Error("AUTH: исключение при проверке логина", exception);
            MessageBox.Show($"Ошибка авторизации:\n{exception.Message}\n\nЛог: {ApplicationLogger.LogPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (User is null) { ApplicationLogger.Info("AUTH: неверный логин или пароль"); MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
        ApplicationLogger.Info($"AUTH: пользователь подтверждён, роль администратора={User.IsAdministrator}");
        DialogResult = true;
    }
}
