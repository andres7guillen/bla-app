using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using Domain.Entities;

namespace Application.Commands.Tasks.CreateTask;

public sealed class CreateTaskCommandHandler
{
    private readonly ITaskRepository _repository;

    public CreateTaskCommandHandler(
        ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var result = TaskItem.Create(
            command.UserId,
            command.Title,
            command.Description,
            command.DueDate);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        await _repository.AddAsync(
            result.Value,
            cancellationToken);

        return Result.Success(result.Value.Id);
    }
}