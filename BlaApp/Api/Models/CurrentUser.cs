using System.Security.Claims;

namespace Api.Models;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(
        IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid UserId =>
        Guid.Parse(
            _accessor.HttpContext!
                .User
                .FindFirstValue(
                    ClaimTypes.NameIdentifier)!);
}
