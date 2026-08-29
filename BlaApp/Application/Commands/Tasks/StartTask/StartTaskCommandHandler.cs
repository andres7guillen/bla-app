using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;

namespace Application.Commands.Tasks.StartTask;

public sealed class StartTaskCommandHandler
{
    private readonly ITaskRepository _repository;

    public StartTaskCommandHandler(
        ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        StartTaskCommand command,
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

        return task.Value.Start();
    }
}
