namespace HrManagement.App.Models;

public sealed class Employee
{
    public int Id { get; set; }

    public string PersonnelNumber { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public DateOnly BirthDate { get; set; }

    public DateOnly HireDate { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public int PositionId { get; set; }

    public Position Position { get; set; } = null!;

    public ICollection<Absence> Absences { get; set; } = new List<Absence>();

    public string FullName => string.Join(
        " ",
        new[] { LastName, FirstName, MiddleName }
            .Where(value => !string.IsNullOrWhiteSpace(value)));
}
