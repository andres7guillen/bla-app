using Api.Models;
using Application.Commands.Tasks.CancelTask;
using Application.Commands.Tasks.CompleteTask;
using Application.Commands.Tasks.CreateTask;
using Application.Commands.Tasks.DeleteTask;
using Application.Commands.Tasks.StartTask;
using Application.Commands.Tasks.UpdateTask;
using Application.Queries.Tasks.GetTaskById;
using Application.Queries.Tasks.GetTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TasksController : ControllerBase
{
    private readonly CreateTaskCommandHandler _createHandler;
    private readonly UpdateTaskCommandHandler _updateHandler;
    private readonly DeleteTaskCommandHandler _deleteHandler;
    private readonly StartTaskCommandHandler _startHandler;
    private readonly CompleteTaskCommandHandler _completeHandler;
    private readonly CancelTaskCommandHandler _cancelHandler;

    private readonly GetTasksQueryHandler _getTasksHandler;
    private readonly GetTaskByIdQueryHandler _getTaskByIdHandler;

    public TasksController(
        CreateTaskCommandHandler createHandler,
        UpdateTaskCommandHandler updateHandler,
        DeleteTaskCommandHandler deleteHandler,
        StartTaskCommandHandler startHandler,
        CompleteTaskCommandHandler completeHandler,
        CancelTaskCommandHandler cancelHandler,
        GetTasksQueryHandler getTasksHandler,
        GetTaskByIdQueryHandler getTaskByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _startHandler = startHandler;
        _completeHandler = completeHandler;
        _cancelHandler = cancelHandler;
        _getTasksHandler = getTasksHandler;
        _getTaskByIdHandler = getTaskByIdHandler;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTasks(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var query = new GetTasksQuery(userId);

        var tasks = await _getTasksHandler.Handle(
            query,
            cancellationToken);

        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var query = new GetTaskByIdQuery(
            id,
            userId);

        var task = await _getTaskByIdHandler.Handle(
            query,
            cancellationToken);

        if (task is null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new CreateTaskCommand(
            userId,
            request.Title,
            request.Description,
            request.DueDate);

        var result = await _createHandler.Handle(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new
            {
                error = result.Error
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.Value
            },
            new
            {
                id = result.Value
            });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var command = new UpdateTaskCommand(
            id,
            userId,
            request.Title,
            request.Description,
            request.DueDate);

        var result = await _updateHandler.Handle(
            command,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Task not found.")
                return NotFound();

            return BadRequest(new
            {
                error = result.Error
            });
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Start(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _startHandler.Handle(
            new StartTaskCommand(
                id,
                GetUserId()),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _completeHandler.Handle(
            new CompleteTaskCommand(
                id,
                GetUserId()),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _cancelHandler.Handle(
            new CancelTaskCommand(
                id,
                GetUserId()),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.Handle(
            new DeleteTaskCommand(
                id,
                GetUserId()),
            cancellationToken);

        return HandleResult(result);
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identity.");
        }

        return userId;
    }

    private IActionResult HandleResult(
        CSharpFunctionalExtensions.Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        if (result.Error == "Task not found.")
            return NotFound();

        return BadRequest(new
        {
            error = result.Error
        });
    }
}
