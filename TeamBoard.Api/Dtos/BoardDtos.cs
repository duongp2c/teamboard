using System.ComponentModel.DataAnnotations;

namespace TeamBoard.Api.Dtos;

public record BoardRequest(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string? Description);

public record BoardResponse(int Id, string Name, string? Description, DateTime CreatedAt, int TaskCount);
