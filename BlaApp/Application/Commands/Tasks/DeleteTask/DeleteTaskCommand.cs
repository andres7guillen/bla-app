namespace Application.Commands.Tasks.DeleteTask;

public sealed record DeleteTaskCommand(
Guid TaskId,
Guid UserId);
