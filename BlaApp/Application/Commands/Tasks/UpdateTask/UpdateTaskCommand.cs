namespace Application.Commands.Tasks.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    Guid UserId,
    string Title,
    string Description,
    DateTime DueDate);
