using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.StartTask;

public sealed class StartTaskCommandHandler : IRequestHandler<StartTaskCommand, Result>
{
    private readonly ITaskRepository _repository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    public StartTaskCommandHandler(
        ITaskRepository repository,
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        StartTaskCommand command,
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

        var result = task.Value.Start();

        if (result.IsFailure)
            return result;

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}
