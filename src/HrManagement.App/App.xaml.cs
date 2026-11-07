using System.Windows;
using HrManagement.App.Data;
using HrManagement.App.Services;
using HrManagement.App.Views;

namespace HrManagement.App;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var connectionString = AppConfiguration.LoadConnectionString();
            var databaseFactory = new DatabaseFactory(connectionString);
            await new DatabaseInitializer(databaseFactory).InitializeAsync();

            var loginWindow = new LoginWindow(new AuthService(databaseFactory));

            if (loginWindow.ShowDialog() != true || loginWindow.Session is null)
            {
                Shutdown();
                return;
            }

            var employeeService = new EmployeeService(databaseFactory);
            var mainWindow = new MainWindow(employeeService, loginWindow.Session);
            MainWindow = mainWindow;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            mainWindow.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Приложение не смогло подключиться к базе данных.\n\n{exception.Message}",
                "Ошибка запуска",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }
}
