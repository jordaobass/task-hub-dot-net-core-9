// DTOs/AuthDtos.cs
namespace TaskHub.DTOs;

public record RegisterRequest(string Email, string Name, string Password, bool AsAdmin = false);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, DateTimeOffset ExpiresAt);