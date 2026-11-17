namespace FleetWise.DAL.Entities.Identity;

public sealed class AdminAuditLog
{
    public long Id { get; set; }
    public string ActorUserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string? ChangeSummary { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}
