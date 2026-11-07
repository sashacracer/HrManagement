namespace HrManagement.Core;

public sealed record StaffingSnapshot(int ActiveEmployees, int Departments, int OpenPositions)
{
    public decimal AverageEmployeesPerDepartment => Departments == 0
        ? 0
        : decimal.Round((decimal)ActiveEmployees / Departments, 1);
}
