using Application.DTOs;
using Application.Interfaces.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Tasks.GetTasks;

public sealed class GetTasksQueryHandler
{
    private readonly IApplicationDbContext _context;

    public GetTasksQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaskResponse>> Handle(
        GetTasksQuery query,
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(x => x.UserId == query.UserId)
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
