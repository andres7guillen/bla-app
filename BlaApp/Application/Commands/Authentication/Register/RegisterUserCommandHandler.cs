using Application.Interfaces.Auth;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Commands.Authentication.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure<Guid>(
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<Guid>(
                "Password is required.");
        }

        return await _identityService.RegisterAsync(
            request.Email.Trim(),
            request.Password);
    }
}
