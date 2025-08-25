// Models/Board.cs
namespace TaskHub.Models;

public class Board
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }

    public List<Column> Columns { get; set; } = new();
}