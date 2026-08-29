using Application.DTOs;
using Application.Interfaces.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Tasks.GetTaskById;

public sealed class GetTaskByIdQueryHandler
{
    private readonly IApplicationDbContext _context;

    public GetTaskByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskResponse?> Handle(
        GetTaskByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(x =>
                x.Id == query.TaskId &&
                x.UserId == query.UserId)
            .Select(x => new TaskResponse(
                x.Id,
                x.Title,
                x.Description,
                x.Status,
                x.DueDate,
                x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
