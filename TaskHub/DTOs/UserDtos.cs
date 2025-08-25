namespace TaskHub.DTOs;

// DTO para listagem de usuários (não expõe PasswordHash)
public record UserListItem(Guid Id, string Email, string Name, string[] Roles);