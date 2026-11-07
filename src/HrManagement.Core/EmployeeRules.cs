using System.Net.Mail;

namespace HrManagement.Core;

public static class EmployeeRules
{
    public static void Validate(EmployeeDraft employee, DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(employee);

        if (string.IsNullOrWhiteSpace(employee.PersonnelNumber))
        {
            throw new ArgumentException("Укажите табельный номер.");
        }

        if (string.IsNullOrWhiteSpace(employee.LastName) ||
            string.IsNullOrWhiteSpace(employee.FirstName))
        {
            throw new ArgumentException("Укажите фамилию и имя сотрудника.");
        }

        var age = CalculateAge(employee.BirthDate, today);

        if (age is < 14 or > 100)
        {
            throw new ArgumentException("Возраст сотрудника должен быть от 14 до 100 лет.");
        }

        if (employee.HireDate > today)
        {
            throw new ArgumentException("Дата приёма не может быть в будущем.");
        }

        if (employee.HireDate < employee.BirthDate.AddYears(14))
        {
            throw new ArgumentException("Дата приёма не соответствует возрасту сотрудника.");
        }

        if (employee.DepartmentId <= 0 || employee.PositionId <= 0)
        {
            throw new ArgumentException("Выберите подразделение и должность.");
        }

        if (!string.IsNullOrWhiteSpace(employee.Email))
        {
            _ = new MailAddress(employee.Email);
        }
    }

    public static int CalculateAge(DateOnly birthDate, DateOnly onDate)
    {
        var age = onDate.Year - birthDate.Year;

        if (birthDate > onDate.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
