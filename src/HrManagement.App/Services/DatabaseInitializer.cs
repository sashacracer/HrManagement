using HrManagement.App.Data;
using HrManagement.App.Models;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.App.Services;

public sealed class DatabaseInitializer(DatabaseFactory databaseFactory)
{
    public async Task InitializeAsync()
    {
        await using var database = databaseFactory.Create();
        await database.Database.EnsureCreatedAsync();

        if (!await database.Departments.AnyAsync())
        {
            await SeedReferenceDataAsync(database);
        }

        if (!await database.UserAccounts.AnyAsync())
        {
            var (hash, salt) = PasswordHasher.Hash("Hr2026!");

            database.UserAccounts.AddRange(
                new UserAccount
                {
                    Login = "hr",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    DisplayName = "Анна Смирнова",
                    Role = "Специалист кадровой службы"
                },
                new UserAccount
                {
                    Login = "admin",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    DisplayName = "Администратор системы",
                    Role = "Администратор"
                });
        }

        await database.SaveChangesAsync();
    }

    private static async Task SeedReferenceDataAsync(AppDbContext database)
    {
        var departments = new[]
        {
            new Department { Code = "ADM", Name = "Администрация" },
            new Department { Code = "HR", Name = "Отдел кадров" },
            new Department { Code = "IT", Name = "Информационные технологии" },
            new Department { Code = "FIN", Name = "Финансовый отдел" },
            new Department { Code = "SALE", Name = "Отдел продаж" }
        };
        var positions = new[]
        {
            new Position { Name = "Директор", SalaryRate = 145_000m },
            new Position { Name = "Специалист по кадрам", SalaryRate = 82_000m },
            new Position { Name = "Системный аналитик", SalaryRate = 115_000m },
            new Position { Name = "Бухгалтер", SalaryRate = 88_000m },
            new Position { Name = "Менеджер по продажам", SalaryRate = 79_000m }
        };

        database.Departments.AddRange(departments);
        database.Positions.AddRange(positions);
        await database.SaveChangesAsync();

        database.Employees.AddRange(
            CreateEmployee("0001", "Смирнова", "Анна", "Викторовна", departments[1], positions[1], new DateOnly(2021, 3, 15), "a.smirnova@company.ru", "+7 900 111-22-33"),
            CreateEmployee("0002", "Волков", "Максим", "Сергеевич", departments[2], positions[2], new DateOnly(2022, 8, 1), "m.volkov@company.ru", "+7 900 222-33-44"),
            CreateEmployee("0003", "Кузнецова", "Елена", "Игоревна", departments[3], positions[3], new DateOnly(2020, 11, 9), "e.kuznetsova@company.ru", "+7 900 333-44-55"),
            CreateEmployee("0004", "Орлов", "Дмитрий", "Андреевич", departments[4], positions[4], new DateOnly(2024, 2, 12), "d.orlov@company.ru", "+7 900 444-55-66"),
            CreateEmployee("0005", "Петрова", "Мария", "Олеговна", departments[0], positions[0], new DateOnly(2019, 5, 20), "m.petrova@company.ru", "+7 900 555-66-77"));

        await database.SaveChangesAsync();

        var employees = await database.Employees.OrderBy(item => item.Id).ToListAsync();
        database.Absences.AddRange(
            new Absence
            {
                EmployeeId = employees[1].Id,
                Type = "Ежегодный отпуск",
                StartDate = new DateOnly(2026, 11, 9),
                EndDate = new DateOnly(2026, 11, 22),
                DocumentNumber = "ОТП-014"
            },
            new Absence
            {
                EmployeeId = employees[3].Id,
                Type = "Командировка",
                StartDate = new DateOnly(2026, 11, 4),
                EndDate = new DateOnly(2026, 11, 6),
                DocumentNumber = "КМД-008"
            });
    }

    private static Employee CreateEmployee(
        string personnelNumber,
        string lastName,
        string firstName,
        string middleName,
        Department department,
        Position position,
        DateOnly hireDate,
        string email,
        string phone)
    {
        return new Employee
        {
            PersonnelNumber = personnelNumber,
            LastName = lastName,
            FirstName = firstName,
            MiddleName = middleName,
            BirthDate = hireDate.AddYears(-28),
            HireDate = hireDate,
            Email = email,
            Phone = phone,
            Department = department,
            Position = position,
            Status = EmploymentStatus.Active
        };
    }
}
