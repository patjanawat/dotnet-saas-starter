namespace SaaS.Domain.Modules.Authorization;

public sealed class RoleDefinition
{
    public Guid Id { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Scope { get; private set; } = string.Empty;
    public bool IsSystem { get; private set; }

    private RoleDefinition() { }

    public static RoleDefinition Create(string key, string name, string scope, bool isSystem = true)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Role key is required.", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Role name is required.", nameof(name));
        }

        return new RoleDefinition
        {
            Id = Guid.NewGuid(),
            Key = key.Trim().ToLowerInvariant(),
            Name = name.Trim(),
            Scope = string.IsNullOrWhiteSpace(scope) ? "tenant" : scope.Trim().ToLowerInvariant(),
            IsSystem = isSystem
        };
    }
}
