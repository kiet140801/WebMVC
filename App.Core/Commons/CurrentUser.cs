using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace App.Core.Commons;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

    public int GetUserId()
    {
        var value = User?.FindFirst("Id")?.Value;
        return int.Parse(value ?? "0");
    }

    public bool IsAdmin()
    {
        var value = User?.FindFirst("IsAdmin")?.Value;
        return bool.Parse(value);
    }

    public string GetUserName()
    {
        return User?.Identity?.Name;
    }

    public IEnumerable<Claim> GetUserClaims()
    {
        return User?.Claims ?? Enumerable.Empty<Claim>();
    }
}
