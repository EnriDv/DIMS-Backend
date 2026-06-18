using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using DIMS_Backend.Features.Auth.Register;
using DIMS_Backend.Features.Auth.Login;

namespace DIMS_Backend.Tests.Behavior;

public class AuthBehaviorTests : IClassFixture<BehaviorTestBase>
{
    private readonly HttpClient _client;

    public AuthBehaviorTests(BehaviorTestBase factory)
    {
        _client = factory.CreateTestClient();
    }

    [Fact]
    public async Task Register_And_Login_Flow_Succeeds()
    {
        // 1. Register a new user
        var registerCommand = new RegisterCommand(
            Nombre: "Comportamiento Test",
            Email: "behavior@ucb.com",
            Password: "SecurePassword123!",
            Rol: "estudiante"
        );

        var registerResponse = await _client.PostAsJsonAsync("api/Auth/register", registerCommand);
        if (registerResponse.StatusCode != HttpStatusCode.OK)
        {
            var content = await registerResponse.Content.ReadAsStringAsync();
            Assert.Fail($"Register failed: {registerResponse.StatusCode} - {content}");
        }

        // 2. Login with the registered user
        var loginCommand = new LoginCommand
        {
            Email = "behavior@ucb.com",
            Password = "SecurePassword123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("api/Auth/login", loginCommand);
        if (loginResponse.StatusCode != HttpStatusCode.OK)
        {
            var content = await loginResponse.Content.ReadAsStringAsync();
            Assert.Fail($"Login failed: {loginResponse.StatusCode} - {content}");
        }

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResultDto>();
        Assert.NotNull(loginResult);
        Assert.NotEmpty(loginResult.AccessToken);
        Assert.Equal("behavior@ucb.com", loginResult.Email);
        Assert.Equal("estudiante", loginResult.Rol);
    }
}
