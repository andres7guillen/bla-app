namespace Application.Queries.Tasks.GetTaskById;

public sealed record GetTaskByIdQuery(
Guid TaskId,
Guid UserId);
