using System.ComponentModel.DataAnnotations;
using TeamBoard.Api.Entities;

namespace TeamBoard.Api.Dtos;

public record TaskRequest(
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    TaskItemStatus Status,
    [MaxLength(100)] string? Assignee,
    DateTime? DueDate,
    [Range(1, int.MaxValue)] int BoardId);

public record TaskResponse(
    int Id, string Title, string? Description, TaskItemStatus Status,
    string? Assignee, DateTime? DueDate, DateTime CreatedAt, int BoardId);
