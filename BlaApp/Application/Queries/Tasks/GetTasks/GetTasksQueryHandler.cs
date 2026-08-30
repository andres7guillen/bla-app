using Application.DTOs;
using Application.Interfaces.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Tasks.GetTasks;

public sealed class GetTasksQueryHandler
    : IRequestHandler<
        GetTasksQuery,
        IReadOnlyList<TaskResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetTasksQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<TaskResponse>> Handle(GetTasksQuery query,CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(x => x.UserId == _currentUser.UserId)
            .OrderBy(x => x.DueDate)
            .Select(x => new TaskResponse(
                x.Id,
                x.Title,
                x.Description,
                x.Status,
                x.DueDate,
                x.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
