namespace Application.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        Guid userId,
        string email);
}