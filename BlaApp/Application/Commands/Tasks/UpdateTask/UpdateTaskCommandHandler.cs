using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.UpdateTask;

public sealed class UpdateTaskCommandHandler
    : IRequestHandler<UpdateTaskCommand, Result>
{
    private readonly ITaskRepository _repository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    public UpdateTaskCommandHandler(
        ITaskRepository repository,
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateTaskCommand command,
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

        var result = task.Value.UpdateDetails(
            command.Title,
            command.Description,
            command.DueDate);

        if (result.IsFailure)
            return result;

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}
