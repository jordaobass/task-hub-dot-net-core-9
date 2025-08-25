// Models/Comment.cs
namespace TaskHub.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public Card? Card { get; set; }

    public Guid AuthorId { get; set; }
    public string Text { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}