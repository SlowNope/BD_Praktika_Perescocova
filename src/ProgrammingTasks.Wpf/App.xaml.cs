using System.Configuration;
using System.Data;
using System.Windows;
using ProgrammingTasks.Infrastructure;

namespace ProgrammingTasks.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ApplicationLogger.Info("START: приложение запущено");
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        ApplicationLogger.Info("AUTH: открытие окна авторизации");
        var login = new LoginWindow();
        if (login.ShowDialog() != true) { ApplicationLogger.Info("AUTH: окно закрыто без успешного входа"); Shutdown(); return; }
        try
        {
            ApplicationLogger.Info($"AUTH: успешный вход, пользователь {login.User?.Login}");
            ApplicationLogger.Info("MAIN: создание главного окна");
            MainWindow = new MainWindow(login.User!);
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            MainWindow.Show();
            ApplicationLogger.Info("MAIN: главное окно показано");
        }
        catch (Exception exception)
        {
            ApplicationLogger.Error("MAIN: ошибка создания главного окна", exception);
            MessageBox.Show($"Не удалось запустить главное окно:\n{exception.Message}\n\nЛог: {ApplicationLogger.LogPath}", "Ошибка запуска", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}

