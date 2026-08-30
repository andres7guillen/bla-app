using Application.DTOs;
using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Tasks.GetTaskHistory;

public sealed class GetTaskHistoryQueryHandler
    : IRequestHandler<
        GetTaskHistoryQuery,
        Maybe<IReadOnlyList<TaskHistoryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    public GetTaskHistoryQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<
        Maybe<IReadOnlyList<TaskHistoryResponse>>>
        Handle(
            GetTaskHistoryQuery query,
            CancellationToken cancellationToken)
    {
        var taskExists = await _context.Tasks
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.Id == query.TaskId &&
                    x.UserId == _currentUser.UserId,
                cancellationToken);

        if (!taskExists)
        {
            return Maybe<IReadOnlyList<TaskHistoryResponse>>
                .None;
        }

        var history = await _context.TaskHistories
            .AsNoTracking()
            .Where(x => x.TaskId == query.TaskId)
            .OrderByDescending(x => x.ChangedAt)
            .Select(x => new TaskHistoryResponse(
                x.Id,
                x.TaskId,
                x.PreviousStatus,
                x.NewStatus,
                x.UserId,
                x.ChangedAt))
            .ToListAsync(cancellationToken);

        return history;
    }
}
