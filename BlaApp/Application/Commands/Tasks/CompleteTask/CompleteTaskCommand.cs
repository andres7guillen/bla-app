namespace Application.Commands.Tasks.CompleteTask;

public sealed record CompleteTaskCommand(
    Guid TaskId,
    Guid UserId);