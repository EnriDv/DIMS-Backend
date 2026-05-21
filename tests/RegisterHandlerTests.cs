using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Features.Auth.Register;
using DIMS_Backend.Infrastructure.Security;

namespace DIMS_Backend.Tests;

public class RegisterHandlerTests
{
    [Fact]
    public async Task Register_Creates_User_In_Database()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Register")
            .Options;

        using var context = new UcbPortalContext(options);
        var hasher = new PasswordHasher();
        var handler = new RegisterCommandHandler(context, hasher);

        var command = new RegisterCommand(
            Nombre: "Test",
            Email: "test@x.com",
            Password: "pass123",
            Rol: "estudiante");

        var result = await handler.Handle(command, default);

        Assert.True(result.IsSuccess);
        var userId = result.Value;
        var user = await context.Users.FindAsync(new object[] { userId });
        Assert.NotNull(user);
        Assert.Equal("test@x.com", user.Email);
    }
}
