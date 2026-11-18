using System.ComponentModel.DataAnnotations;
using ClaimsWebApi.Domain;

namespace ClaimsWebApi.Request
{
    public class UpdateClaimStatusRequest
    {
        [Required(ErrorMessage = "NewStatus is required.")]
        [EnumDataType(typeof(ClaimStatus), ErrorMessage = "NewStatus must be a valid ClaimStatus value.")]
        public ClaimStatus? NewStatus { get; set; }
    }
}
