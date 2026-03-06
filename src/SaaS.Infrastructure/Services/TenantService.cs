using Microsoft.EntityFrameworkCore;
using SaaS.Application.Common;
using SaaS.Application.Contracts;
using SaaS.Application.Tenant;
using SaaS.Domain.Modules.Tenant;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Services;

public sealed class TenantService : ITenantService, ICreateTenantCommandHandler, IGetTenantByIdQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public TenantService(AppDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<ApplicationResult<TenantResponse>> CreateTenantAsync(CreateTenantCommand command, CancellationToken cancellationToken)
        => await HandleAsync(command, cancellationToken);

    public async Task<ApplicationResult<TenantResponse>> HandleAsync(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Code) || string.IsNullOrWhiteSpace(command.Name))
        {
            return ApplicationResult<TenantResponse>.Failure("tenant.invalid_input", "Tenant code and name are required.", 400);
        }

        if (!TenantSlug.TryNormalize(command.Code, out var normalizedCode))
        {
            return ApplicationResult<TenantResponse>.Failure("tenant.invalid_slug", "Tenant code must be a lowercase slug (letters, digits, hyphen).", 400);
        }

        var exists = await _dbContext.Tenants.AnyAsync(x => x.Code == normalizedCode, cancellationToken);
        if (exists)
        {
            return ApplicationResult<TenantResponse>.Failure("tenant.duplicate_code", "Tenant code already exists.", 409);
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var tenant = Tenant.Create(command.Code, command.Name, now);
        _dbContext.Tenants.Add(tenant);

        _dbContext.TenantConfigurations.Add(TenantConfiguration.Create(tenant.Id, "timezone", "UTC", now));
        _dbContext.TenantConfigurations.Add(TenantConfiguration.Create(tenant.Id, "locale", "en-US", now));

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResult<TenantResponse>.Success(new TenantResponse(tenant.Id, tenant.Code, tenant.Name, tenant.Status.ToString()));
    }

    public async Task<ApplicationResult<TenantResponse>> HandleAsync(GetTenantByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.TenantId == Guid.Empty)
        {
            return ApplicationResult<TenantResponse>.Failure("tenant.invalid_id", "Tenant ID is required.", 400);
        }

        var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(x => x.Id == query.TenantId, cancellationToken);
        if (tenant is null)
        {
            return ApplicationResult<TenantResponse>.Failure("tenant.not_found", "Tenant not found.", 404);
        }

        return ApplicationResult<TenantResponse>.Success(
            new TenantResponse(tenant.Id, tenant.Code, tenant.Name, tenant.Status.ToString()));
    }
}
