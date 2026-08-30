using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Persistance;

public interface IApplicationDbContext
{
    DbSet<TaskItem> Tasks { get; }
    DbSet<TaskHistory> TaskHistories { get; }
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}
