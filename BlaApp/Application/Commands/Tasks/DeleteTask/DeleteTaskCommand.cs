using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.DeleteTask;

public sealed record DeleteTaskCommand(Guid TaskId): IRequest<Result>;
