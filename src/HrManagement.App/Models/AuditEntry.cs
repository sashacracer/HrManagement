namespace HrManagement.App.Models;

public sealed class AuditEntry
{
    public long Id { get; set; }

    public int? UserAccountId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public DateTime CreatedAt { get; set; }
}
