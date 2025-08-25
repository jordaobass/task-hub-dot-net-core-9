using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TaskHub.Data;
using TaskHub.DTOs;

namespace TaskHub.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsersEndpoints(this RouteGroupBuilder api)
    {
        // Apenas administradores podem listar usuários
        var group = api.MapGroup("/users").RequireAuthorization("AdminOnly");

        group.MapGet("/", async Task<Ok<List<UserListItem>>> (AppDbContext db) =>
        {
            var users = await db.Users
                .Select(u => new UserListItem(u.Id, u.Email, u.Name, u.Roles))
                .ToListAsync();

            return TypedResults.Ok(users);
        });

        return group;
    }
}