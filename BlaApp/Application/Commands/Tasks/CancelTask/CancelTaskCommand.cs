using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.CancelTask;

public sealed record CancelTaskCommand(Guid TaskId): IRequest<Result>;
