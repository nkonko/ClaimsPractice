
using ClaimsWebApi.Domain;
using ClaimsWebApi.Request;
using ClaimsWebApi.Responses;
using ClaimsWebApi.Result;

namespace ClaimsWebApi.Servicies
{
    public interface IClaimService
    {
        Task<IEnumerable<Claim>> GetClaimsAsync(CancellationToken cancellationToken);

        Task RegisterClaimAsync(Claim claim, CancellationToken cancellationToken);

        Task<Claim?> GetClaimByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<UpdateClaimStatusResult> UpdateClaimStatusAsync(Guid id, ClaimStatus newStatus, CancellationToken cancellationToken);

        Task<List<Claim>> SearchClaimAsync(SearchClaimsRequest request, CancellationToken cancellationToken);

        Task<List<PolicyClaimsSummary>> GetPolicyClaimsSummaryAsync(CancellationToken cancellationToken);
    }
}
