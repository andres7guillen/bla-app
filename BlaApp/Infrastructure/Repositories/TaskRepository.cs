using Application.Interfaces.Persistance;
using CSharpFunctionalExtensions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Maybe<TaskItem>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken)
    {
        await _context.Tasks.AddAsync(
            task,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public void Remove(TaskItem task)
    {
        _context.Tasks.Remove(task);
    }
}
