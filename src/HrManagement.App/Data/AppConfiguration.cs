using System.IO;
using System.Text.Json;

namespace HrManagement.App.Data;

public static class AppConfiguration
{
    public static string LoadConnectionString()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        var json = File.ReadAllText(path);
        using var document = JsonDocument.Parse(json);

        return document.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("HrManagementDb")
            .GetString()
            ?? throw new InvalidOperationException("Строка подключения HrManagementDb не задана.");
    }
}
