using Application.Commands.Tasks.CreateTask;
using Application.Interfaces.Persistance;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Tests.Application.Tests.Commands.Tasks.CreateTask;

public sealed class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;

    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _currentUserMock = new Mock<ICurrentUser>();

        _handler = new CreateTaskCommandHandler(
            _repositoryMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateTaskSuccessfully()
    {
        // Arrange

        var userId = Guid.NewGuid();

        _currentUserMock
            .Setup(x => x.UserId)
            .Returns(userId);

        var command = new CreateTaskCommand(
            "Test task",
            "Test description",
            DateTime.UtcNow.AddDays(1));

        _repositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<global::Domain.Entities.TaskItem>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBe(Guid.Empty);

        _repositoryMock.Verify(
            x => x.AddAsync(
                It.Is<global::Domain.Entities.TaskItem>(
                    task =>
                        task.UserId == userId &&
                        task.Title == "Test task" &&
                        task.Description == "Test description" &&
                        task.DueDate == command.DueDate &&
                        task.Status == global::Domain.Enums.TaskStatus.Pending),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _currentUserMock.Verify(
            x => x.UserId,
            Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTaskCreationFails()
    {
        // Arrange

        var userId = Guid.NewGuid();

        _currentUserMock
            .Setup(x => x.UserId)
            .Returns(userId);

        // Aquí usamos datos que violen una regla
        // de negocio definida en TaskItem.Create().
        var command = new CreateTaskCommand(
            "",
            "Test description",
            DateTime.UtcNow.AddDays(1));

        // Act

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.IsFailure.Should().BeTrue();

        result.Error.Should().NotBeNullOrWhiteSpace();

        _repositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<global::Domain.Entities.TaskItem>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task Handle_ShouldUseCurrentUserId_WhenCreatingTask()
    {
        // Arrange

        var userId = Guid.NewGuid();

        _currentUserMock
            .Setup(x => x.UserId)
            .Returns(userId);

        var command = new CreateTaskCommand(
            "Test task",
            "Test description",
            DateTime.UtcNow.AddDays(1));

        global::Domain.Entities.TaskItem? createdTask = null;

        _repositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<global::Domain.Entities.TaskItem>(),
                It.IsAny<CancellationToken>()))
            .Callback<global::Domain.Entities.TaskItem, CancellationToken>(
                (task, _) => createdTask = task)
            .Returns(Task.CompletedTask);

        // Act

        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        createdTask.Should().NotBeNull();

        createdTask!.UserId.Should().Be(userId);
    }


    [Fact]
    public async Task Handle_ShouldReturnCreatedTaskId()
    {
        // Arrange

        var userId = Guid.NewGuid();

        _currentUserMock
            .Setup(x => x.UserId)
            .Returns(userId);

        var command = new CreateTaskCommand(
            "Test task",
            "Test description",
            DateTime.UtcNow.AddDays(1));

        global::Domain.Entities.TaskItem? createdTask = null;

        _repositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<global::Domain.Entities.TaskItem>(),
                It.IsAny<CancellationToken>()))
            .Callback<global::Domain.Entities.TaskItem, CancellationToken>(
                (task, _) => createdTask = task)
            .Returns(Task.CompletedTask);

        // Act

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.IsSuccess.Should().BeTrue();

        createdTask.Should().NotBeNull();

        result.Value.Should().Be(createdTask!.Id);
    }
}
