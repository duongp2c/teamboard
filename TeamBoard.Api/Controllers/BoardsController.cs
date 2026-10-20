using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamBoard.Api.Data;
using TeamBoard.Api.Dtos;
using TeamBoard.Api.Entities;

namespace TeamBoard.Api.Controllers;

[ApiController]
[Route("api/boards")]
public class BoardsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BoardsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoardResponse>>> GetAll() =>
        await _db.Boards
            .OrderBy(b => b.Id)
            .Select(b => new BoardResponse(b.Id, b.Name, b.Description, b.CreatedAt, b.Tasks.Count))
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BoardResponse>> GetById(int id)
    {
        var board = await _db.Boards
            .Where(b => b.Id == id)
            .Select(b => new BoardResponse(b.Id, b.Name, b.Description, b.CreatedAt, b.Tasks.Count))
            .FirstOrDefaultAsync();
        return board is null ? NotFound() : board;
    }

    [HttpGet("{id:int}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks(int id)
    {
        if (!await _db.Boards.AnyAsync(b => b.Id == id)) return NotFound();
        return await _db.Tasks
            .Where(t => t.BoardId == id)
            .OrderBy(t => t.Id)
            .Select(t => new TaskResponse(t.Id, t.Title, t.Description, t.Status, t.Assignee, t.DueDate, t.CreatedAt, t.BoardId))
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<BoardResponse>> Create(BoardRequest req)
    {
        var board = new Board { Name = req.Name, Description = req.Description };
        _db.Boards.Add(board);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = board.Id },
            new BoardResponse(board.Id, board.Name, board.Description, board.CreatedAt, 0));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BoardRequest req)
    {
        var board = await _db.Boards.FindAsync(id);
        if (board is null) return NotFound();
        board.Name = req.Name;
        board.Description = req.Description;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var board = await _db.Boards.FindAsync(id);
        if (board is null) return NotFound();
        _db.Boards.Remove(board); // tasks are cascade-deleted
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
