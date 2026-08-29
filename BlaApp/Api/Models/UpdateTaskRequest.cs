namespace Api.Models;

public sealed record UpdateTaskRequest(
    string Title,
    string Description,
    DateTime DueDate);
