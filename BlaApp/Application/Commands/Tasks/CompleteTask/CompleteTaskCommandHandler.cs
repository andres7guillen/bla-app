using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tasks.CompleteTask;

public sealed class CompleteTaskCommandHandler
    : IRequestHandler<CompleteTaskCommand, Result>
{
    private readonly ITaskRepository _repository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    public CompleteTaskCommandHandler(
        ITaskRepository repository,
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        CompleteTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(
            command.TaskId,
            cancellationToken);

        if (task.Value is null ||
            task.Value.UserId != _currentUser.UserId)
        {
            return Result.Failure(
                "Task not found.");
        }
        var previousStatus = task.Value.Status;
        var result = task.Value.Complete();

        if (result.IsFailure)
            return result;

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
