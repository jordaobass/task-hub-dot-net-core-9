// Endpoints/AuthEndpoints.cs
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TaskHub.Data;
using TaskHub.DTOs;
using TaskHub.Models;
using TaskHub.Utils;

namespace TaskHub.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/auth");

        group.MapPost("/register", async Task<Results<Ok<AuthResponse>, ValidationProblem>> (
            RegisterRequest req, AppDbContext db, JwtTokenService jwt) =>
        {
            var exists = await db.Users.AnyAsync(u => u.Email == req.Email);
            if (exists)
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["email"] = new[] { "E-mail já cadastrado" }
                });

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = req.Email,
                Name = req.Name,
                PasswordHash = req.Password, // DEMO
                Roles = req.AsAdmin ? new[] { "admin" } : Array.Empty<string>()
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var (token, exp) = jwt.GenerateToken(user);
            return TypedResults.Ok(new AuthResponse(token, exp));
        });

        group.MapPost("/login", async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>> (
            LoginRequest req, AppDbContext db, JwtTokenService jwt) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user is null || user.PasswordHash != req.Password) // DEMO
                return TypedResults.Unauthorized();

            var (token, exp) = jwt.GenerateToken(user);
            return TypedResults.Ok(new AuthResponse(token, exp));
        });

        return group;
    }
}