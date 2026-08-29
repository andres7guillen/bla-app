using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;

namespace Application.Commands.Tasks.CompleteTask;

public sealed class CompleteTaskCommandHandler
{
    private readonly ITaskRepository _repository;

    public CompleteTaskCommandHandler(
        ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        CompleteTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(
            command.TaskId,
            cancellationToken);

        if (task.Value is null)
            return Result.Failure("Task not found.");

        if (task.Value.UserId != command.UserId)
            return Result.Failure("Task not found.");

        return task.Value.Complete();
    }
}
