namespace Api.Models;

public sealed record CreateTaskRequest(
    string Title,
    string Description,
    DateTime DueDate);
