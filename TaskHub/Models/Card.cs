// Models/Card.cs
namespace TaskHub.Models;

public class Card
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public Guid? AssigneeId { get; set; }

    public Guid ColumnId { get; set; }
    public Column? Column { get; set; }

    public List<Label> Labels { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}