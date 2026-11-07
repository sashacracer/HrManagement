namespace HrManagement.App.Models;

public sealed class Position
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal SalaryRate { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
