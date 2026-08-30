using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tasks.CreateTask;

public sealed class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskRepository _repository;
    private readonly ICurrentUser _currentUser;
    public CreateTaskCommandHandler(
        ITaskRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var result = TaskItem.Create(
            _currentUser.UserId,
            command.Title,
            command.Description,
            command.DueDate);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(
                result.Error);
        }

        await _repository.AddAsync(
            result.Value,
            cancellationToken);

        return Result.Success(
            result.Value.Id);
    }
}