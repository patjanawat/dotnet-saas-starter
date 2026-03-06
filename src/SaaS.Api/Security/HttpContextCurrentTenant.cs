using SaaS.Application.Contracts;

namespace SaaS.Api.Security;

public sealed class HttpContextCurrentTenant : ICurrentTenant
{
    private const string TenantIdHeader = "X-Tenant-Id";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentTenant(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
    {
        get
        {
            var headers = _httpContextAccessor.HttpContext?.Request.Headers;
            if (headers is null || !headers.TryGetValue(TenantIdHeader, out var value))
            {
                return null;
            }

            return Guid.TryParse(value.ToString(), out var tenantId) ? tenantId : null;
        }
    }

    public bool HasTenant => TenantId.HasValue;
}
