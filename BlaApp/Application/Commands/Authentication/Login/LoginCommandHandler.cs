using Application.Interfaces.Auth;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Authentication.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<string>>
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
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result =
            await _identityService.ValidateCredentialsAsync(
                command.Email,
                command.Password);

        if (result.IsFailure)
        {
            return Result.Failure<string>(
                "Invalid credentials.");
        }

        var token = _jwtTokenGenerator.GenerateToken(
            result.Value,
            command.Email);

        return Result.Success(token);
    }
}
