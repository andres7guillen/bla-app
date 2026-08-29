using Application.Interfaces.Auth;
using CSharpFunctionalExtensions;

namespace Application.Commands.Authentication.Register;

public sealed class RegisterUserCommandHandler
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return Result.Failure<Guid>(
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return Result.Failure<Guid>(
                "Password is required.");
        }

        return await _identityService.RegisterAsync(
            command.Email.Trim(),
            command.Password);
    }
}
