using CSharpFunctionalExtensions;

namespace Domain.Entities;

public sealed class TaskItem
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public Enums.TaskStatus Status { get; private set; }

    public DateTime DueDate { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
    public ICollection<TaskHistory> History { get; private set; } = new List<TaskHistory>();

    private TaskItem(){}

    private TaskItem(
        Guid id,
        Guid userId,
        string title,
        string description,
        DateTime dueDate)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = Enums.TaskStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<TaskItem> Create(
        Guid userId,
        string title,
        string description,
        DateTime dueDate)
    {
        if (userId == Guid.Empty)
            return Result.Failure<TaskItem>(
                "User is required.");

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<TaskItem>(
                "Title is required.");

        title = title.Trim();

        if (title.Length > 100)
            return Result.Failure<TaskItem>(
                "Title cannot exceed 100 characters.");

        description ??= string.Empty;
        description = description.Trim();

        if (description.Length > 500)
            return Result.Failure<TaskItem>(
                "Description cannot exceed 500 characters.");

        if (dueDate <= DateTime.UtcNow)
            return Result.Failure<TaskItem>(
                "Due date must be in the future.");

        return Result.Success(
            new TaskItem(
                Guid.NewGuid(),
                userId,
                title,
                description,
                dueDate));
    }

    public Result UpdateDetails(
        string title,
        string description,
        DateTime dueDate)
    {
        if (Status is Enums.TaskStatus.Completed or Enums.TaskStatus.Cancelled)
            return Result.Failure(
                "Completed or cancelled tasks cannot be modified.");

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(
                "Title is required.");

        title = title.Trim();

        if (title.Length > 100)
            return Result.Failure(
                "Title cannot exceed 100 characters.");

        description ??= string.Empty;
        description = description.Trim();

        if (description.Length > 500)
            return Result.Failure(
                "Description cannot exceed 500 characters.");

        if (dueDate <= DateTime.UtcNow)
            return Result.Failure(
                "Due date must be in the future.");

        Title = title;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Start()
    {
        if (Status != Enums.TaskStatus.Pending)
            return Result.Failure(
                "Only pending tasks can be started.");

        Status = Enums.TaskStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != Enums.TaskStatus.InProgress)
            return Result.Failure(
                "Only tasks in progress can be completed.");

        Status = Enums.TaskStatus.Completed;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == Enums.TaskStatus.Completed)
            return Result.Failure(
                "Completed tasks cannot be cancelled.");

        if (Status == Enums.TaskStatus.Cancelled)
            return Result.Failure(
                "Task is already cancelled.");

        Status = Enums.TaskStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
