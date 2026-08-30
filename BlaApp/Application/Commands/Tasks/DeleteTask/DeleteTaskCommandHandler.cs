using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;

namespace Application.Commands.Tasks.DeleteTask;

public sealed class DeleteTaskCommandHandler
{
    private readonly ITaskRepository _repository;
    private readonly ICurrentUser _currentUser;
    public DeleteTaskCommandHandler(
        ITaskRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(
            command.TaskId,
            cancellationToken);

        if (task.Value is null ||
            task.Value.UserId != _currentUser.UserId)
        {
            return Result.Failure("Task not found.");
        }

        _repository.Remove(task.Value);

        return Result.Success();
    }
}
