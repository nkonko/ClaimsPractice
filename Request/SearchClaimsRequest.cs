using System.ComponentModel.DataAnnotations;
using ClaimsWebApi.Domain;

namespace ClaimsWebApi.Request
{
    public class SearchClaimsRequest
    {
        public string? PolicyNumber { get; set; }

        [EnumDataType(typeof(ClaimStatus), ErrorMessage = "Status is not valid.")]
        public ClaimStatus? Status { get; set; }

        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }
}
