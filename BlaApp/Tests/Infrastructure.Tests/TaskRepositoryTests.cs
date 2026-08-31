using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure.Tests;

public sealed class TaskRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddTaskAndSaveChanges()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context =
            new ApplicationDbContext(options);

        var repository = new TaskRepository(context);

        var userId = Guid.NewGuid();

        var taskResult = TaskItem.Create(
            userId,
            "Test task",
            "Test description",
            DateTime.UtcNow.AddDays(1));

        taskResult.IsSuccess.Should().BeTrue();

        var task = taskResult.Value;

        // Act

        await repository.AddAsync(
            task,
            CancellationToken.None);

        // Assert

        var savedTask = await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == task.Id);

        savedTask.Should().NotBeNull();

        savedTask!.Id.Should().Be(task.Id);
        savedTask.UserId.Should().Be(userId);
        savedTask.Title.Should().Be("Test task");
        savedTask.Description.Should().Be("Test description");

        savedTask.Status
            .Should()
            .Be(global::Domain.Enums.TaskStatus.Pending);
    }
}
