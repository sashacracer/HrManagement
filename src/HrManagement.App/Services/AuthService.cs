using HrManagement.App.Data;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.App.Services;

public sealed class AuthService(DatabaseFactory databaseFactory)
{
    public async Task<UserSession?> SignInAsync(string login, string password)
    {
        await using var database = databaseFactory.Create();
        var normalizedLogin = login.Trim().ToLowerInvariant();
        var account = await database.UserAccounts
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Login == normalizedLogin);

        if (account is null || !account.IsActive)
        {
            return null;
        }

        if (!PasswordHasher.Verify(password, account.PasswordHash, account.PasswordSalt))
        {
            return null;
        }

        return new UserSession(account.Id, account.DisplayName, account.Role);
    }
}
