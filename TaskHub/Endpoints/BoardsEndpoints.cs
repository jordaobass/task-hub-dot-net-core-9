// Endpoints/BoardsEndpoints.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskHub.Data;
using TaskHub.DTOs;
using TaskHub.Models;
using TaskHub.Utils;

namespace TaskHub.Endpoints;

public static class BoardsEndpoints
{
    public static RouteGroupBuilder MapBoardsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/boards").RequireAuthorization();

        // Listar boards do usuário autenticado
        group.MapGet("/", async Task<Ok<List<BoardResponse>>> (ClaimsPrincipal user, AppDbContext db) =>
        {
            var userId = GetUserId(user);
            var boards = await db.Boards
                .Where(b => b.OwnerId == userId)
                .Select(b => BoardResponse.From(b))
                .ToListAsync();

            return TypedResults.Ok(boards);
        });

        // Criar board
        group.MapPost("/", async Task<Results<Created<BoardResponse>, ValidationProblem>> (
            ClaimsPrincipal user, BoardCreateRequest req, AppDbContext db) =>
        {
            var entity = new Board
            {
                Id = Guid.NewGuid(),
                Title = req.Title,
                Description = req.Description,
                OwnerId = GetUserId(user)
            };

            db.Boards.Add(entity);
            await db.SaveChangesAsync();

            var resp = BoardResponse.From(entity);
            return TypedResults.Created($"/api/v1/boards/{entity.Id}", resp);
        })
        .AddEndpointFilterFactory((ctx, next) =>
        {
            // Resolva o validator no escopo da requisição, não no ApplicationServices (root).
            return async invocationContext =>
            {
                var validator = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<FluentValidation.IValidator<BoardCreateRequest>>();
                var filter = new ValidationFilter<BoardCreateRequest>(validator);
                return await filter.InvokeAsync(invocationContext, next);
            };
        });

        // Detalhar board (inclui colunas e cards)
        group.MapGet("/{id:guid}", async Task<Results<Ok<Board>, NotFound>> (Guid id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var userId = GetUserId(user);
            var board = await db.Boards
                .Include(b => b.Columns.OrderBy(c => c.Order))
                    .ThenInclude(c => c.Cards)
                .FirstOrDefaultAsync(b => b.Id == id && b.OwnerId == userId);

            return board is null ? TypedResults.NotFound() : TypedResults.Ok(board);
        });

        // Atualizar board
        group.MapPut("/{id:guid}", async Task<Results<NoContent, NotFound, ValidationProblem>> (
            Guid id, ClaimsPrincipal user, BoardUpdateRequest req, AppDbContext db) =>
        {
            var userId = GetUserId(user);
            var board = await db.Boards.FirstOrDefaultAsync(b => b.Id == id && b.OwnerId == userId);
            if (board is null) return TypedResults.NotFound();

            board.Title = req.Title;
            board.Description = req.Description;
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .AddEndpointFilterFactory((ctx, next) =>
        {
            // ... existing code ...
            return async invocationContext =>
            {
                var validator = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<FluentValidation.IValidator<BoardUpdateRequest>>();
                var filter = new ValidationFilter<BoardUpdateRequest>(validator);
                return await filter.InvokeAsync(invocationContext, next);
            };
        });

        // Remover board (somente admin)
        group.MapDelete("/{id:guid}", [Authorize(Policy = "AdminOnly")] async Task<Results<NoContent, NotFound>> (
            Guid id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var userId = GetUserId(user);
            var board = await db.Boards.FirstOrDefaultAsync(b => b.Id == id && b.OwnerId == userId);
            if (board is null) return TypedResults.NotFound();

            db.Boards.Remove(board);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        });

        // Criar coluna em um board
        group.MapPost("/{boardId:guid}/columns", async Task<Results<Created<Column>, NotFound, ValidationProblem>> (
            Guid boardId, ClaimsPrincipal user, ColumnCreateRequest req, AppDbContext db) =>
        {
            var userId = GetUserId(user);
            var board = await db.Boards.Include(b => b.Columns)
                .FirstOrDefaultAsync(b => b.Id == boardId && b.OwnerId == userId);
            if (board is null) return TypedResults.NotFound();

            var order = req.Order ?? (board.Columns.Count == 0 ? 1 : board.Columns.Max(c => c.Order) + 1);
            var col = new Column
            {
                Id = Guid.NewGuid(),
                Title = req.Title,
                Order = order,
                BoardId = board.Id
            };
            db.Columns.Add(col);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/api/v1/boards/{boardId}/columns/{col.Id}", col);
        })
        .AddEndpointFilterFactory((ctx, next) =>
        {
            // Resolva o validator no RequestServices
            return async invocationContext =>
            {
                var validator = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<FluentValidation.IValidator<ColumnCreateRequest>>();
                var filter = new ValidationFilter<ColumnCreateRequest>(validator);
                return await filter.InvokeAsync(invocationContext, next);
            };
        });

        // Criar card numa coluna
        group.MapPost("/{boardId:guid}/columns/{columnId:guid}/cards", async Task<Results<Created<Card>, NotFound, ValidationProblem>> (
            Guid boardId, Guid columnId, ClaimsPrincipal user, CardCreateRequest req, AppDbContext db) =>
        {
            var userId = GetUserId(user);
            var column = await db.Columns.Include(c => c.Board)
                .FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId && c.Board!.OwnerId == userId);
            if (column is null) return TypedResults.NotFound();

            var card = new Card
            {
                Id = Guid.NewGuid(),
                Title = req.Title,
                Description = req.Description,
                DueDate = req.DueDate,
                AssigneeId = req.AssigneeId,
                ColumnId = column.Id
            };
            db.Cards.Add(card);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/api/v1/boards/{boardId}/columns/{columnId}/cards/{card.Id}", card);
        })
        .AddEndpointFilterFactory((ctx, next) =>
        {
            // ... existing code ...
            return async invocationContext =>
            {
                var validator = invocationContext.HttpContext.RequestServices
                    .GetRequiredService<FluentValidation.IValidator<CardCreateRequest>>();
                var filter = new ValidationFilter<CardCreateRequest>(validator);
                return await filter.InvokeAsync(invocationContext, next);
            };
        });

        // Comentar card
        group.MapPost("/{boardId:guid}/cards/{cardId:guid}/comments", async Task<Results<Created<Comment>, NotFound, ValidationProblem>> (
            Guid boardId, Guid cardId, ClaimsPrincipal user, CommentCreateRequest req, AppDbContext db) =>
        {
            var userId = GetUserId(user);
            var card = await db.Cards
                .Include(c => c.Column!).ThenInclude(col => col.Board!)
                .FirstOrDefaultAsync(c => c.Id == cardId && c.Column!.Board!.Id == boardId && c.Column.Board.OwnerId == userId);
            if (card is null) return TypedResults.NotFound();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                CardId = card.Id,
                AuthorId = userId,
                Text = req.Text,
                CreatedAt = DateTimeOffset.UtcNow
            };

            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/api/v1/boards/{boardId}/cards/{cardId}/comments/{comment.Id}", comment);
        });

        return group;
    }

    private static Guid GetUserId(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub");
        return sub is null ? Guid.Empty : Guid.Parse(sub);
    }
}