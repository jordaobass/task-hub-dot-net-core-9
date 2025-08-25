// DTOs/BoardDtos.cs
using TaskHub.Models;

namespace TaskHub.DTOs;

public record BoardCreateRequest(string Title, string? Description);
public record BoardUpdateRequest(string Title, string? Description);

public record BoardResponse(Guid Id, string Title, string? Description, Guid OwnerId)
{
    public static BoardResponse From(Board b) =>
        new(b.Id, b.Title, b.Description, b.OwnerId);
}