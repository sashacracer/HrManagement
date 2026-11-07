using HrManagement.App.Models;

namespace HrManagement.App.ViewModels;

public sealed class EmployeeRowViewModel(Employee employee)
{
    public Employee Employee { get; } = employee;

    public int Id => Employee.Id;

    public string PersonnelNumber => Employee.PersonnelNumber;

    public string FullName => Employee.FullName;

    public string Department => Employee.Department.Name;

    public string Position => Employee.Position.Name;

    public string HireDate => Employee.HireDate.ToString("dd.MM.yyyy");

    public string Status => Employee.Status switch
    {
        EmploymentStatus.Active => "Работает",
        EmploymentStatus.OnLeave => "Отсутствует",
        EmploymentStatus.Dismissed => "Уволен",
        _ => "Не указан"
    };
}
