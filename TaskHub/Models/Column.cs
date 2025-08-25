// Models/Column.cs
namespace TaskHub.Models;

public class Column
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int Order { get; set; }

    public Guid BoardId { get; set; }
    public Board? Board { get; set; }

    public List<Card> Cards { get; set; } = new();
}