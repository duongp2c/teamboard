using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamBoard.Api.Data;
using TeamBoard.Api.Dtos;
using TeamBoard.Api.Entities;

namespace TeamBoard.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db) => _db = db;

    private static TaskResponse ToResponse(TaskItem t) =>
        new(t.Id, t.Title, t.Description, t.Status, t.Assignee, t.DueDate, t.CreatedAt, t.BoardId);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll([FromQuery] int? boardId, [FromQuery] TaskItemStatus? status)
    {
        var query = _db.Tasks.AsNoTracking();
        if (boardId.HasValue) query = query.Where(t => t.BoardId == boardId);
        if (status.HasValue) query = query.Where(t => t.Status == status);
        var tasks = await query.OrderBy(t => t.Id).ToListAsync();
        return tasks.Select(ToResponse).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetById(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        return task is null ? NotFound() : ToResponse(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(TaskRequest req)
    {
        if (!await _db.Boards.AnyAsync(b => b.Id == req.BoardId))
            return BadRequest(new { message = $"Board {req.BoardId} does not exist" });

        var task = new TaskItem
        {
            Title = req.Title,
            Description = req.Description,
            Status = req.Status,
            Assignee = req.Assignee,
            DueDate = req.DueDate,
            BoardId = req.BoardId
        };
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, ToResponse(task));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskRequest req)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        if (task.BoardId != req.BoardId && !await _db.Boards.AnyAsync(b => b.Id == req.BoardId))
            return BadRequest(new { message = $"Board {req.BoardId} does not exist" });

        task.Title = req.Title;
        task.Description = req.Description;
        task.Status = req.Status;
        task.Assignee = req.Assignee;
        task.DueDate = req.DueDate;
        task.BoardId = req.BoardId;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
