using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.CancelTask;

public sealed class CancelTaskCommandHandler
    : IRequestHandler<CancelTaskCommand, Result>
{
    private readonly ITaskRepository _repository;
    private readonly IApplicationDbContext _context;

    public CancelTaskCommandHandler(
        ITaskRepository repository,
        IApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<Result> Handle(
        CancelTaskCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(
            command.TaskId,
            cancellationToken);

        if (task.Value is null)
        {
            return Result.Failure(
                "Task not found.");
        }

        var result = task.Value.Cancel();

        if (result.IsFailure)
            return result;

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}
