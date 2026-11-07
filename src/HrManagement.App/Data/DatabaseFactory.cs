using Microsoft.EntityFrameworkCore;

namespace HrManagement.App.Data;

public sealed class DatabaseFactory(string connectionString)
{
    public AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure())
            .Options;

        return new AppDbContext(options);
    }
}
