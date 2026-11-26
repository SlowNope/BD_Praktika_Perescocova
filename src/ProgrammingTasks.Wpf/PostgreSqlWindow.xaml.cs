using System.Windows;
using ProgrammingTasks.Infrastructure;

namespace ProgrammingTasks.Wpf;

public partial class PostgreSqlWindow : Window
{
    public PostgreSqlConnectionSettings? Settings { get; private set; }
    public PostgreSqlWindow()
    {
        InitializeComponent(); var settings = new PostgreSqlSettingsStore().Load(); HostText.Text = settings.Host; PortText.Text = settings.Port.ToString(); DatabaseText.Text = settings.Database; UsernameText.Text = settings.Username; PasswordText.Password = settings.Password; KeyDown += (_, args) => { if (args.Key == System.Windows.Input.Key.Escape) { args.Handled = true; Close(); } };
    }
    private async void ConnectClick(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PortText.Text, out var port) || port is < 1 or > 65535 || string.IsNullOrWhiteSpace(HostText.Text) || string.IsNullOrWhiteSpace(DatabaseText.Text) || string.IsNullOrWhiteSpace(UsernameText.Text)) { MessageBox.Show("Заполните сервер, порт, базу данных и пользователя.", "Проверка данных"); return; }
        var settings = new PostgreSqlConnectionSettings { Host = HostText.Text.Trim(), Port = port, Database = DatabaseText.Text.Trim(), Username = UsernameText.Text.Trim(), Password = PasswordText.Password };
        try { await new PostgreSqlMigrationService().TestAndMigrateAsync(settings, [], CancellationToken.None); new PostgreSqlSettingsStore().Save(settings); Settings = settings; MessageBox.Show("Подключение успешно. Настройки сохранены.", "PostgreSQL"); DialogResult = true; } catch (Exception exception) { ApplicationLogger.Error("POSTGRES: ошибка подключения из интерфейса", exception); MessageBox.Show($"Не удалось подключиться:\n{exception.Message}", "PostgreSQL", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
    private void CancelClick(object sender, RoutedEventArgs e) => DialogResult = false;
}
