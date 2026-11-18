
using Microsoft.EntityFrameworkCore;
using ClaimsWebApi.Data;
using ClaimsWebApi.Domain;
using ClaimsWebApi.Request;
using ClaimsWebApi.Responses;
using ClaimsWebApi.Result;

namespace ClaimsWebApi.Servicies.Imp
{
    public class ClaimService : IClaimService
    {

        private readonly ClaimsDbContext _dbContext;
        private readonly ILogger<ClaimService> _logger;

        public ClaimService(ClaimsDbContext dbContext, ILogger<ClaimService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Claim?> GetClaimByIdAsync(Guid id, CancellationToken cancellationToken)
        {
           return await _dbContext.Claims.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Claims.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task RegisterClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            _dbContext.Claims.Add(claim);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<UpdateClaimStatusResult> UpdateClaimStatusAsync(Guid id, ClaimStatus newStatus, CancellationToken cancellationToken)
        {
            var claim = await _dbContext.Claims.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (claim == null)
            {
                return UpdateClaimStatusResult.NotFound;
            }

            if (!claim.ChangeStatus(newStatus))
            {
                return UpdateClaimStatusResult.InvalidTransition;
            }

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException exception)
            {
                _logger.LogWarning(exception, "Concurrency conflict updating status for claim {ClaimId}.", id);
                return UpdateClaimStatusResult.ConcurrencyConflict;
            }

            return UpdateClaimStatusResult.Success;
        }

        public async Task<List<Claim>> SearchClaimAsync(SearchClaimsRequest request, CancellationToken cancellationToken)
        {
            long offset = ((long)request.Page - 1) * request.PageSize;

            var query =
                _dbContext.Claims
                .AsNoTracking()
                .AsQueryable();

            if (request.Status.HasValue)
            {
                query = query.Where(c => c.Status == request.Status.Value);
            }

            if (!string.IsNullOrEmpty(request.PolicyNumber))
            {
                query = query.Where(c => c.PolicyNumber == request.PolicyNumber);
            }

            if (request.FromUtc.HasValue)
            {
                query = query.Where(c => c.OccurredAtUtc >= request.FromUtc.Value);
            }

            if (request.ToUtc.HasValue)
            {
                query = query.Where(c => c.OccurredAtUtc < request.ToUtc.Value);
            }



            return await query
                .OrderByDescending(c => c.OccurredAtUtc)
                .ThenBy(c => c.Id)
                .Skip(checked((int)offset))
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }

        public Task<List<PolicyClaimsSummary>> GetPolicyClaimsSummaryAsync(CancellationToken cancellationToken)
        {
            return _dbContext.Claims.AsNoTracking()
                .GroupBy(c => c.PolicyNumber)
                .Select(c => new PolicyClaimsSummary
                {
                    PolicyNumber= c.Key,
                    TotalClaims = c.Count(),
                    ApprovedClaims = c.Count(claim => claim.Status == ClaimStatus.Approved),
                    LastOccurredAtUtc = c.Max(claim => claim.OccurredAtUtc)
                })
                .OrderByDescending(c => c.TotalClaims)
                .ThenBy(c => c.PolicyNumber)
                .ToListAsync(cancellationToken);
        }
    }
}
