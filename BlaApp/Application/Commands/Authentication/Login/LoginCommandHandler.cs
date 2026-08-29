using Application.Interfaces.Auth;
using CSharpFunctionalExtensions;

namespace Application.Commands.Authentication.Login;

public sealed class LoginCommandHandler
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<string>> Handle(
        LoginCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return Result.Failure<string>(
                "Invalid credentials.");
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return Result.Failure<string>(
                "Invalid credentials.");
        }

        var result =
            await _identityService.ValidateCredentialsAsync(
                command.Email.Trim(),
                command.Password);

        if (result.IsFailure)
        {
            return Result.Failure<string>(
                result.Error);
        }

        var token = _jwtTokenGenerator.GenerateToken(
            result.Value,
            command.Email.Trim());

        return Result.Success(token);
    }
}
