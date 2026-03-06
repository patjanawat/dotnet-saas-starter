using System.Text.RegularExpressions;

namespace SaaS.Domain.Modules.Tenant;

public static partial class TenantSlug
{
    public static bool TryNormalize(string raw, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var candidate = raw.Trim().ToLowerInvariant();
        if (!SlugRegex().IsMatch(candidate))
        {
            return false;
        }

        normalized = candidate;
        return true;
    }

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex SlugRegex();
}
