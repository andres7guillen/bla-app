using Application.DTOs;
using MediatR;

namespace Application.Queries.Tasks.GetTasks;

public sealed record GetTasksQuery
    : IRequest<IReadOnlyList<TaskResponse>>;
