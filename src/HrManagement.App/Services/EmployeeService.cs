using HrManagement.App.Data;
using HrManagement.App.Models;
using HrManagement.Core;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.App.Services;

public sealed class EmployeeService(DatabaseFactory databaseFactory)
{
    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync(string? search)
    {
        await using var database = databaseFactory.Create();
        var query = database.Employees
            .AsNoTracking()
            .Include(item => item.Department)
            .Include(item => item.Position)
            .Where(item => item.Status != EmploymentStatus.Dismissed);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(item =>
                item.LastName.Contains(value) ||
                item.FirstName.Contains(value) ||
                item.PersonnelNumber.Contains(value) ||
                item.Department.Name.Contains(value));
        }

        return await query
            .OrderBy(item => item.LastName)
            .ThenBy(item => item.FirstName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Department>> GetDepartmentsAsync()
    {
        await using var database = databaseFactory.Create();
        return await database.Departments.AsNoTracking().OrderBy(item => item.Name).ToListAsync();
    }

    public async Task<IReadOnlyList<Position>> GetPositionsAsync()
    {
        await using var database = databaseFactory.Create();
        return await database.Positions.AsNoTracking().OrderBy(item => item.Name).ToListAsync();
    }

    public async Task SaveAsync(EmployeeEditModel model, UserSession session)
    {
        var draft = new EmployeeDraft(
            model.PersonnelNumber.Trim(),
            model.LastName.Trim(),
            model.FirstName.Trim(),
            model.MiddleName?.Trim(),
            DateOnly.FromDateTime(model.BirthDate),
            DateOnly.FromDateTime(model.HireDate),
            model.Email.Trim(),
            model.Phone.Trim(),
            model.DepartmentId,
            model.PositionId);

        EmployeeRules.Validate(draft, DateOnly.FromDateTime(DateTime.Today));

        await using var database = databaseFactory.Create();
        var employee = model.Id.HasValue
            ? await database.Employees.SingleAsync(item => item.Id == model.Id.Value)
            : new Employee();

        var duplicateExists = await database.Employees.AnyAsync(item =>
            item.Id != employee.Id &&
            (item.PersonnelNumber == draft.PersonnelNumber || item.Email == draft.Email));

        if (duplicateExists)
        {
            throw new ArgumentException("Табельный номер или адрес электронной почты уже используется.");
        }

        employee.PersonnelNumber = draft.PersonnelNumber;
        employee.LastName = draft.LastName;
        employee.FirstName = draft.FirstName;
        employee.MiddleName = draft.MiddleName;
        employee.BirthDate = draft.BirthDate;
        employee.HireDate = draft.HireDate;
        employee.Email = draft.Email;
        employee.Phone = draft.Phone;
        employee.DepartmentId = draft.DepartmentId;
        employee.PositionId = draft.PositionId;
        employee.Status = EmploymentStatus.Active;

        if (!model.Id.HasValue)
        {
            database.Employees.Add(employee);
        }

        database.AuditEntries.Add(new AuditEntry
        {
            UserAccountId = session.Id,
            Action = model.Id.HasValue ? "Изменение сотрудника" : "Добавление сотрудника",
            EntityName = nameof(Employee),
            EntityId = model.Id?.ToString(),
            CreatedAt = DateTime.Now
        });

        await database.SaveChangesAsync();
    }

    public async Task DismissAsync(int employeeId, UserSession session)
    {
        await using var database = databaseFactory.Create();
        var employee = await database.Employees.SingleAsync(item => item.Id == employeeId);
        employee.Status = EmploymentStatus.Dismissed;

        database.AuditEntries.Add(new AuditEntry
        {
            UserAccountId = session.Id,
            Action = "Увольнение сотрудника",
            EntityName = nameof(Employee),
            EntityId = employee.Id.ToString(),
            CreatedAt = DateTime.Now
        });

        await database.SaveChangesAsync();
    }

    public async Task<(int Employees, int Departments, int Absences)> GetDashboardAsync()
    {
        await using var database = databaseFactory.Create();
        var today = DateOnly.FromDateTime(DateTime.Today);
        var employees = await database.Employees.CountAsync(item => item.Status != EmploymentStatus.Dismissed);
        var departments = await database.Departments.CountAsync();
        var absences = await database.Absences.CountAsync(item => item.StartDate <= today && item.EndDate >= today);

        return (employees, departments, absences);
    }
}
