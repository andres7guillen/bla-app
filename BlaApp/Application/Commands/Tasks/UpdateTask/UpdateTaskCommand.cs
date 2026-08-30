using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string Description,
    DateTime DueDate) : IRequest<Result>;