using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.CompleteTask;

public sealed record CompleteTaskCommand(
    Guid TaskId): IRequest<Result>;