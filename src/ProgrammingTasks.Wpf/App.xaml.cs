using System.Configuration;
using System.Data;
using System.Windows;

namespace ProgrammingTasks.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var login = new LoginWindow();
        if (login.ShowDialog() != true) { Shutdown(); return; }
        try
        {
            MainWindow = new MainWindow(login.User!);
            MainWindow.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Не удалось запустить главное окно:\n{exception.Message}", "Ошибка запуска", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}

