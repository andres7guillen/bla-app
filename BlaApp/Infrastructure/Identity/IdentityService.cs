using Application.Interfaces.Auth;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<Guid>> RegisterAsync(
        string email,
        string password)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return Result.Failure<Guid>(
                "User already exists.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(
            user,
            password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(x => x.Description));

            return Result.Failure<Guid>(errors);
        }

        return Result.Success(user.Id);
    }

    public async Task<Result<Guid>> ValidateCredentialsAsync(
        string email,
        string password)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure<Guid>(
                "Invalid credentials.");
        }

        var validPassword =
            await _userManager.CheckPasswordAsync(
                user,
                password);

        if (!validPassword)
        {
            return Result.Failure<Guid>(
                "Invalid credentials.");
        }

        return Result.Success(user.Id);
    }

    public async Task<string?> GetEmailAsync(
        Guid userId)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        return user?.Email;
    }
}
