using CSharpFunctionalExtensions;
using Domain.Entities;

namespace Application.Interfaces.Persistance;

public interface ITaskRepository
{
    Task<Maybe<TaskItem>> GetByIdAsync(Guid id,CancellationToken cancellationToken);

    Task AddAsync(TaskItem task,CancellationToken cancellationToken);

    void Remove(TaskItem task);
}
