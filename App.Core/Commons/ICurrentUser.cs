using System.Security.Claims;

namespace App.Core.Commons;

public interface ICurrentUser
{
    int GetUserId();
    string GetUserName();
    IEnumerable<Claim> GetUserClaims();
    bool IsAdmin();
}
