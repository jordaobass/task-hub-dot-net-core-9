// Models/User.cs
namespace TaskHub.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string PasswordHash { get; set; } = default!; // DEMO: plain-text
    public string[] Roles { get; set; } = Array.Empty<string>();
}