using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;

namespace Application.Commands.Tasks.UpdateTask;

public sealed class UpdateTaskCommandHandler
{
    private readonly ITaskRepository _repository;

    public UpdateTaskCommandHandler(
        ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        UpdateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(
            command.TaskId,
            cancellationToken);

        if (task.Value is null)
            return Result.Failure("Task not found.");

        if (task.Value.UserId != command.UserId)
            return Result.Failure("Task not found.");

        var result = task.Value.UpdateDetails(
            command.Title,
            command.Description,
            command.DueDate);

        return result;
    }
}
