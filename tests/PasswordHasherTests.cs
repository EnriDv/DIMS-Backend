using Xunit;
using DIMS_Backend.Infrastructure.Security;

namespace DIMS_Backend.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void HashAndVerify_Works()
    {
        var hasher = new PasswordHasher();
        var pwd = "My$ecret123";
        var hash = hasher.Hash(pwd);
        Assert.True(hasher.Verify(pwd, hash));
        Assert.False(hasher.Verify("wrong", hash));
    }
}
