using HrManagement.Core;

namespace HrManagement.Tests;

public sealed class EmployeeRulesTests
{
    private static readonly DateOnly Today = new(2026, 11, 2);

    [Fact]
    public void ValidEmployeeDoesNotThrow()
    {
        var employee = CreateValidEmployee();

        EmployeeRules.Validate(employee, Today);
    }

    [Fact]
    public void EmptyPersonnelNumberIsRejected()
    {
        var employee = CreateValidEmployee() with { PersonnelNumber = "" };

        Assert.Throws<ArgumentException>(() => EmployeeRules.Validate(employee, Today));
    }

    [Fact]
    public void MissingNameIsRejected()
    {
        var employee = CreateValidEmployee() with { FirstName = " " };

        Assert.Throws<ArgumentException>(() => EmployeeRules.Validate(employee, Today));
    }

    [Fact]
    public void EmployeeYoungerThanFourteenIsRejected()
    {
        var employee = CreateValidEmployee() with { BirthDate = new DateOnly(2014, 1, 1) };

        Assert.Throws<ArgumentException>(() => EmployeeRules.Validate(employee, Today));
    }

    [Fact]
    public void FutureHireDateIsRejected()
    {
        var employee = CreateValidEmployee() with { HireDate = Today.AddDays(1) };

        Assert.Throws<ArgumentException>(() => EmployeeRules.Validate(employee, Today));
    }

    [Fact]
    public void InvalidEmailIsRejected()
    {
        var employee = CreateValidEmployee() with { Email = "wrong-email" };

        Assert.Throws<FormatException>(() => EmployeeRules.Validate(employee, Today));
    }

    [Fact]
    public void AgeBeforeBirthdayIsCalculatedCorrectly()
    {
        var age = EmployeeRules.CalculateAge(new DateOnly(2000, 12, 1), Today);

        Assert.Equal(25, age);
    }

    [Fact]
    public void AgeAfterBirthdayIsCalculatedCorrectly()
    {
        var age = EmployeeRules.CalculateAge(new DateOnly(2000, 1, 1), Today);

        Assert.Equal(26, age);
    }

    private static EmployeeDraft CreateValidEmployee()
    {
        return new EmployeeDraft(
            "0042",
            "Иванов",
            "Иван",
            "Иванович",
            new DateOnly(1995, 4, 18),
            new DateOnly(2024, 2, 1),
            "i.ivanov@company.ru",
            "+7 900 000-00-00",
            1,
            1);
    }
}
