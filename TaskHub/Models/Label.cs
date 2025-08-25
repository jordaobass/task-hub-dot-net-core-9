// Models/Label.cs
namespace TaskHub.Models;

public class Label
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = "#cccccc";
}