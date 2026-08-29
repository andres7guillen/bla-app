namespace Application.Commands.Tasks.CancelTask;

public sealed record CancelTaskCommand(
Guid TaskId,
Guid UserId);
