// DTOs/CardDtos.cs
using System.ComponentModel;

namespace TaskHub.DTOs;

public record ColumnCreateRequest(string Title, int? Order);
public record CardCreateRequest(string Title, string? Description, DateTimeOffset? DueDate, Guid? AssigneeId);
public record CardMoveRequest(Guid TargetColumnId, int? Position);
public record CommentCreateRequest(string Text);