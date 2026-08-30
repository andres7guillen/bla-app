using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    string Description,
    DateTime DueDate)
    : IRequest<Result<Guid>>;