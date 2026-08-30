using Application.DTOs;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Queries.Tasks.GetTaskHistory;

public sealed record GetTaskHistoryQuery(
Guid TaskId)
: IRequest<Maybe<IReadOnlyList<TaskHistoryResponse>>>;
