using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;

namespace Application.Commands.Tasks.DeleteTask;

public sealed class DeleteTaskCommandHandler
{
    private readonly ITaskRepository _repository;

    public DeleteTaskCommandHandler(
        ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        DeleteTaskCommand command,
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

        _repository.Remove(task.Value);

        return Result.Success();
    }
}
