using Microsoft.EntityFrameworkCore;
using TaskHub.Data;
using TaskHub.Models;

namespace TaskHub.Data;

public static class DbSeeder
{
    // Seed simples para banco InMemory, com alguns dados de exemplo
    public static void Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Se já houver usuários, assumimos que o seed foi executado
        if (db.Users.Any())
            return;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@taskhub.local",
            Name = "Admin",
            PasswordHash = "admin",
            Roles = new[] { "admin" }
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@taskhub.local",
            Name = "User",
            PasswordHash = "1234",
            Roles = Array.Empty<string>()
        };

        db.Users.AddRange(admin, user);

        // Board de exemplo para o admin
        var adminBoard = new Board
        {
            Id = Guid.NewGuid(),
            Title = "Board do Admin",
            Description = "Exemplo inicial",
            OwnerId = admin.Id,
            Columns = new List<Column>()
        };

        var todo = new Column
        {
            Id = Guid.NewGuid(),
            Title = "A Fazer",
            Order = 1,
            BoardId = adminBoard.Id,
            Cards = new List<Card>()
        };

        var doing = new Column
        {
            Id = Guid.NewGuid(),
            Title = "Fazendo",
            Order = 2,
            BoardId = adminBoard.Id,
            Cards = new List<Card>()
        };

        var done = new Column
        {
            Id = Guid.NewGuid(),
            Title = "Feito",
            Order = 3,
            BoardId = adminBoard.Id,
            Cards = new List<Card>()
        };

        adminBoard.Columns.AddRange([todo, doing, done]);

        var card1 = new Card
        {
            Id = Guid.NewGuid(),
            Title = "Configurar autenticação JWT",
            Description = "Adicionar endpoints de login/registro e geração de token",
            DueDate = DateTimeOffset.UtcNow.AddDays(3),
            ColumnId = todo.Id,
            Labels = new List<Label>
            {
                new Label { Id = Guid.NewGuid(), Name = "Backend", Color = "#6E44FF" },
                new Label { Id = Guid.NewGuid(), Name = "Auth", Color = "#FF6F59" }
            },
            Comments = new List<Comment>()
        };

        var card2 = new Card
        {
            Id = Guid.NewGuid(),
            Title = "Criar endpoint de boards",
            Description = "CRUD básico com Minimal API",
            ColumnId = doing.Id,
            Comments = new List<Comment>
            {
                new Comment
                {
                    Id = Guid.NewGuid(),
                    AuthorId = admin.Id,
                    Text = "Lembrar de usar TypedResults e validação com FluentValidation"
                }
            }
        };

        todo.Cards.Add(card1);
        doing.Cards.Add(card2);

        db.Boards.Add(adminBoard);

        // Board vazio para o usuário comum
        var userBoard = new Board
        {
            Id = Guid.NewGuid(),
            Title = "Projetos Pessoais",
            Description = "Tarefas do usuário",
            OwnerId = user.Id
        };

        db.Boards.Add(userBoard);

        db.SaveChanges();
    }
}