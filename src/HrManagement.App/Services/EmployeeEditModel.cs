namespace HrManagement.App.Services;

public sealed class EmployeeEditModel
{
    public int? Id { get; set; }

    public string PersonnelNumber { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-25);

    public DateTime HireDate { get; set; } = DateTime.Today;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public int PositionId { get; set; }
}
