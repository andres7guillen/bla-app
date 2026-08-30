using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tasks.CancelTask;

public sealed class CancelTaskCommandHandler
    : IRequestHandler<CancelTaskCommand, Result>
{
    private readonly ITaskRepository _repository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    public CancelTaskCommandHandler(
        ITaskRepository repository,
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        CancelTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(
            command.TaskId,
            cancellationToken);

        if (task.Value is null)
        {
            return Result.Failure(
                "Task not found.");
        }

        var result = task.Value.Cancel();

        if (result.IsFailure)
            return result;

        var previousStatus = task.Value.Status;

        var historyResult =
            TaskHistory.Create(
                task.Value.Id,
                previousStatus.ToString(),
                task.Value.Status.ToString(),
                _currentUser.UserId);

        await _context.TaskHistories.AddAsync(
            historyResult,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}
