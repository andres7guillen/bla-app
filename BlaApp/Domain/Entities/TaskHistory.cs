namespace Domain.Entities;

public sealed class TaskHistory
{
    private TaskHistory()
    {
    }

    private TaskHistory(
        Guid id,
        Guid taskId,
        string? previousStatus,
        string newStatus,
        Guid userId,
        DateTime changedAt)
    {
        Id = id;
        TaskId = taskId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        UserId = userId;
        ChangedAt = changedAt;
    }

    public Guid Id { get; private set; }

    public Guid TaskId { get; private set; }

    public string? PreviousStatus { get; private set; }

    public string NewStatus { get; private set; } = null!;

    public Guid UserId { get; private set; }

    public DateTime ChangedAt { get; private set; }

    public static TaskHistory Create(
        Guid taskId,
        string? previousStatus,
        string newStatus,
        Guid userId)
    {
        return new TaskHistory(
            Guid.NewGuid(),
            taskId,
            previousStatus,
            newStatus,
            userId,
            DateTime.UtcNow);
    }
}