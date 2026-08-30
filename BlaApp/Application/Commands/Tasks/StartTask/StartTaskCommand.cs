using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.StartTask;

public sealed record StartTaskCommand(Guid TaskId): IRequest<Result>;
