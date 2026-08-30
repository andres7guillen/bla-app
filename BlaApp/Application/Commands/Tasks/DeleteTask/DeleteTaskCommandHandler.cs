using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Tasks.DeleteTask;

public sealed class DeleteTaskCommandHandler
    : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly ITaskRepository _repository;
    private readonly IApplicationDbContext _context;

    public DeleteTaskCommandHandler(
        ITaskRepository repository,
        IApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteTaskCommand command,
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

        _repository.Remove(task.Value);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}
