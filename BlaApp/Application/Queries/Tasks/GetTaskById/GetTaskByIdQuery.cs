using Application.DTOs;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Queries.Tasks.GetTaskById;

public sealed record GetTaskByIdQuery(
    Guid TaskId)
    : IRequest<Maybe<TaskResponse>>;
