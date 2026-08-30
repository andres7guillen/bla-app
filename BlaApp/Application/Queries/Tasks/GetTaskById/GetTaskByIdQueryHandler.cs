using Application.DTOs;
using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Tasks.GetTaskById;

public sealed class GetTaskByIdQueryHandler
    : IRequestHandler<GetTaskByIdQuery, Maybe<TaskResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    public GetTaskByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Maybe<TaskResponse>> Handle(
        GetTaskByIdQuery query,
        CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .Where(x =>
                x.Id == query.TaskId &&
                x.UserId == _currentUser.UserId)
            .Select(x => new TaskResponse(
                x.Id,
                x.Title,
                x.Description,
                x.Status,
                x.DueDate,
                x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return task != null 
            ? Maybe<TaskResponse>.From(task) 
            : Maybe<TaskResponse>.None;
    }
}