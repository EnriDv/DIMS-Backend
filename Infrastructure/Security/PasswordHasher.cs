namespace DIMS_Backend.Infrastructure.Security;

using BCrypt.Net;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        // Genera un hash seguro con un "salt" automático
        return BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        // Compara la contraseña en texto plano con el hash de la base de datos
        return BCrypt.Verify(password, passwordHash);
    }
}