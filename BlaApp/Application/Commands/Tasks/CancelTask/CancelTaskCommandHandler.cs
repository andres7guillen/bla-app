using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;

namespace Application.Commands.Tasks.CancelTask;

public sealed class CancelTaskCommandHandler
{
    private readonly ITaskRepository _repository;

    public CancelTaskCommandHandler(
        ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        CancelTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(
            command.TaskId,
            cancellationToken);

        if (task.Value is null ||
            task.Value.UserId != command.UserId)
        {
            return Result.Failure("Task not found.");
        }

        return task.Value.Cancel();
    }
}
