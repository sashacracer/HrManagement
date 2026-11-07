namespace HrManagement.Core;

public sealed record EmployeeDraft(
    string PersonnelNumber,
    string LastName,
    string FirstName,
    string? MiddleName,
    DateOnly BirthDate,
    DateOnly HireDate,
    string Email,
    string Phone,
    int DepartmentId,
    int PositionId);
