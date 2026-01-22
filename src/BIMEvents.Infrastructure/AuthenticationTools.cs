using System.Security.Claims;
using BIMEvents.Domain;
using Microsoft.AspNetCore.Http;

namespace BIMEvents.Infrastructure;

public static class AuthenticationTools
{
    public static MetaData GetAccount(this IHttpContextAccessor httpContextAccessor)
    {
        if (!httpContextAccessor.HttpContext!.User.Identity!.IsAuthenticated)
            throw new UnauthorizedAccessException();

        var id = Guid.Parse(httpContextAccessor.HttpContext.User.FindFirst("id")!.Value);
        var name = httpContextAccessor.HttpContext.User.FindFirst("name")!.Value;
        var username = httpContextAccessor.HttpContext.User.FindFirst("preferred_username")!.Value;
        var email = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;

        return new MetaData(id, name, username, email);
    }
}