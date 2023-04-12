using System.Diagnostics;
using Avangardum.LifeArena.Server.Interfaces;

namespace Avangardum.LifeArena.Server.Helpers;

public class IpAddressUserIdProvider : IUserIdProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IpAddressUserIdProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            Debug.Assert(context != null);
            var ipAddress = context.Connection.RemoteIpAddress;
            return ipAddress != null ? ipAddress.ToString() : string.Empty;
        }
    }
}