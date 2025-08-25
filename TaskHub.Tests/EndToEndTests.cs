using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TaskHub.Tests;

public record AuthResponse(string Token, DateTimeOffset ExpiresAt);

public class EndToEndTests
{
    [Fact]
    public async Task Register_Login_CreateBoard_Flow_Should_Work_In_Isolated_InMemory_Db()
    {
        await using var factory = new TaskHubApiFactory();
        using var client = factory.CreateClient();

        // 1) Registrar usuário
        var registerPayload = new
        {
            email = "user1@example.com",
            name = "User 1",
            password = "123456",
            asAdmin = false
        };
        var regResp = await client.PostAsJsonAsync("/api/v1/auth/register", registerPayload);
        regResp.EnsureSuccessStatusCode();

        // 2) Login
        var loginPayload = new { email = "user1@example.com", password = "123456" };
        var loginResp = await client.PostAsJsonAsync("/api/v1/auth/login", loginPayload);
        loginResp.EnsureSuccessStatusCode();

        var auth = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();
        auth.Should().NotBeNull();
        auth!.Token.Should().NotBeNullOrWhiteSpace();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth.Token);

        // 3) Criar board
        var createBoardPayload = new { title = "Board A", description = "Desc A" };
        var createBoardResp = await client.PostAsJsonAsync("/api/v1/boards", createBoardPayload);
        createBoardResp.EnsureSuccessStatusCode();

        var createdBoard = await createBoardResp.Content.ReadFromJsonAsync<dynamic>();
        ((string)createdBoard.title).Should().Be("Board A");

        // 4) Listar boards
        var listResp = await client.GetAsync("/api/v1/boards");
        listResp.EnsureSuccessStatusCode();

        var list = await listResp.Content.ReadFromJsonAsync<dynamic[]>();
        list.Should().NotBeNull();
        list!.Length.Should().Be(1);
    }

    [Fact]
    public async Task Each_Test_Should_Have_Its_Own_Database_Instance()
    {
        // Primeiro teste/fábrica
        await using var factory1 = new TaskHubApiFactory();
        using var client1 = factory1.CreateClient();

        // Segundo teste/fábrica (DB diferente)
        await using var factory2 = new TaskHubApiFactory();
        using var client2 = factory2.CreateClient();

        // No factory1 registramos um usuário
        var reg1 = await client1.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email = "iso1@example.com",
            name = "Iso 1",
            password = "123456",
            asAdmin = false
        });
        reg1.EnsureSuccessStatusCode();

        // No factory2 registramos outro usuário de mesmo email (deve funcionar pois DB é diferente)
        var reg2 = await client2.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email = "iso1@example.com",
            name = "Iso 1 - Other DB",
            password = "123456",
            asAdmin = false
        });
        reg2.EnsureSuccessStatusCode();
    }
}