namespace Application.Commands.Tasks.StartTask;

public sealed record StartTaskCommand(
  Guid TaskId,
  Guid UserId);
