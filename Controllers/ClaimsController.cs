using Microsoft.AspNetCore.Mvc;
using ClaimsWebApi.Domain;
using ClaimsWebApi.Request;
using ClaimsWebApi.Result;
using ClaimsWebApi.Servicies;

namespace ClaimsWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimService _claimService;
        public ClaimsController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SearchClaimsRequest request, CancellationToken cancellationToken)
        {
            var offset = ((long)request.Page - 1) * request.PageSize;

            if(offset > int.MaxValue)
            {
                ModelState.AddModelError(nameof(SearchClaimsRequest.Page), "Page number is too large.");
            }

            if(request.FromUtc.HasValue && request.ToUtc.HasValue && request.FromUtc >= request.ToUtc)
            {
                ModelState.AddModelError( nameof(request.ToUtc), "ToUtc must be greater than FromUtc.");    
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var claims = await _claimService.SearchClaimAsync(request, cancellationToken); 

            return Ok(claims) ;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterClaim(RegisterClaimRequest claim, CancellationToken cancellationToken)
        {
            if(claim.OccurredAtUtc == default)
            {
                ModelState.AddModelError(nameof(RegisterClaimRequest.OccurredAtUtc), "OccurredAtUtc is required and must be a valid date.");
            }
            else if(claim.OccurredAtUtc.Kind != DateTimeKind.Utc)
            {
                ModelState.AddModelError(nameof(RegisterClaimRequest.OccurredAtUtc), "OccurredAtUtc must be in UTC format.");
            }
            else if(claim.OccurredAtUtc > DateTime.UtcNow)
            {
                ModelState.AddModelError(nameof(RegisterClaimRequest.OccurredAtUtc), "OccurredAtUtc cannot be in the future.");
            }
            
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var newClaim = new Claim
            {
                Id = Guid.NewGuid(),
                PolicyNumber = claim.PolicyNumber,
                Description = claim.Description,
                OccurredAtUtc = claim.OccurredAtUtc,
            };

            await _claimService.RegisterClaimAsync(newClaim, cancellationToken);


            return CreatedAtAction(nameof(GetClaimById), new { id = newClaim.Id }, newClaim);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClaimById(Guid id, CancellationToken cancellationToken)
        {
            if(id == Guid.Empty)
            {
                return NotFound();
            }

            var claim = await _claimService.GetClaimByIdAsync(id, cancellationToken);

            if(claim == null)
            {
                return NotFound();
            }

            return Ok(claim);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateClaimStatus(Guid id, UpdateClaimStatusRequest request, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var result = await _claimService.UpdateClaimStatusAsync(id, request.NewStatus.Value, cancellationToken);

            return result switch
            {
                UpdateClaimStatusResult.Success => NoContent(),
                UpdateClaimStatusResult.NotFound => NotFound(),
                UpdateClaimStatusResult.InvalidTransition => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "The requested transition is not allowed from the current status."),
                UpdateClaimStatusResult.ConcurrencyConflict => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "The claim was modified or deleted by another request. Reload it before trying again."),
                _ => throw new InvalidOperationException($"Unexpected claim status update result: {result}.")
            };
        }

        [HttpGet("reports/by-policy")]
        public async Task<IActionResult> GetPolicyClaimsSummary(CancellationToken cancellationToken)
        {
            var summary = await _claimService.GetPolicyClaimsSummaryAsync(cancellationToken);
            return Ok(summary);
        }
    }
}
