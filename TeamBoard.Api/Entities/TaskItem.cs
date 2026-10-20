namespace TeamBoard.Api.Entities;

public enum TaskItemStatus
{
    Todo,
    InProgress,
    Done
}

// Named TaskItem to avoid clashing with System.Threading.Tasks.Task; mapped to table "Tasks"
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public string? Assignee { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int BoardId { get; set; }
    public Board? Board { get; set; }
}
