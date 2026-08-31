using Api.Controllers;
using Api.Models;
using Application.Commands.Tasks.CreateTask;
using CSharpFunctionalExtensions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Api.Controllers.TaskController;

public sealed class TasksControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();

        _controller = new TasksController(
            _mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenTaskIsCreated()
    {
        // Arrange

        var taskId = Guid.NewGuid();

        var request = new CreateTaskRequest(
            "Test task",
            "Test description",
            DateTime.UtcNow.AddDays(1));

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateTaskCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success(taskId));

        // Act

        var result = await _controller.Create(
            request,
            CancellationToken.None);

        // Assert

        var createdResult = result
            .Should()
            .BeOfType<CreatedAtActionResult>()
            .Subject;

        createdResult.ActionName
            .Should()
            .Be(nameof(TasksController.GetById));

        createdResult.RouteValues
            .Should()
            .ContainKey("id");

        createdResult.RouteValues!["id"]
            .Should()
            .Be(taskId);

        createdResult.Value
            .Should()
            .BeEquivalentTo(new
            {
                id = taskId
            });

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<CreateTaskCommand>(
                    command =>
                        command.Title == "Test task" &&
                        command.Description == "Test description" &&
                        command.DueDate == request.DueDate),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCommandFails()
    {
        // Arrange

        var request = new CreateTaskRequest(
            "Test task",
            "Test description",
            DateTime.UtcNow.AddDays(1)
        );

        const string error = "Unable to create task.";

        _mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateTaskCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<Guid>(error));

        // Act

        var result = await _controller.Create(
            request,
            CancellationToken.None);

        // Assert

        var badRequestResult = result
            .Should()
            .BeOfType<BadRequestObjectResult>()
            .Subject;

        badRequestResult.Value
            .Should()
            .Be(error);

        _mediatorMock.Verify(
            x => x.Send(
                It.IsAny<CreateTaskCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
