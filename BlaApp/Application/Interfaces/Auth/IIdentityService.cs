using CSharpFunctionalExtensions;

namespace Application.Interfaces.Auth;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(
        string email,
        string password);

    Task<Result<Guid>> ValidateCredentialsAsync(
        string email,
        string password);

    Task<string?> GetEmailAsync(
        Guid userId);
}
