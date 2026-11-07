using HrManagement.Core;

namespace HrManagement.Tests;

public sealed class StaffingSnapshotTests
{
    [Fact]
    public void AverageEmployeesPerDepartmentIsRoundedToOneDigit()
    {
        var snapshot = new StaffingSnapshot(10, 3, 2);

        Assert.Equal(3.3m, snapshot.AverageEmployeesPerDepartment);
    }

    [Fact]
    public void EmptyDepartmentListReturnsZeroAverage()
    {
        var snapshot = new StaffingSnapshot(0, 0, 0);

        Assert.Equal(0m, snapshot.AverageEmployeesPerDepartment);
    }
}
