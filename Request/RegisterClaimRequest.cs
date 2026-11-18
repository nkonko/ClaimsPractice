
using System.ComponentModel.DataAnnotations;

namespace ClaimsWebApi.Request
{
    public class RegisterClaimRequest
    {
        [Required(ErrorMessage = "Policy number is required.")]
        public required string PolicyNumber { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public required string Description { get; set; }

        public DateTime OccurredAtUtc { get; set; }

    }
}
