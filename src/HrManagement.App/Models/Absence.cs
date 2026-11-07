namespace HrManagement.App.Models;

public sealed class Absence
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public string Type { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? DocumentNumber { get; set; }

    public string? Comment { get; set; }
}
