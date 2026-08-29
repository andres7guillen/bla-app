using Domain.Enums;

namespace Application.DTOs;

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string Description,
    Domain.Enums.TaskStatus Status,
    DateTime DueDate,
    DateTime CreatedAt);
