using System.ComponentModel.DataAnnotations;

namespace ClaimsWebApi.Domain
{
    public class Claim
    {
        public Guid Id { get; set; }

        public required string PolicyNumber { get; set; }

        public required string Description { get; set; }

        public DateTime OccurredAtUtc { get; set; }

        public ClaimStatus Status { get; private set; } = ClaimStatus.Reported;

        [Timestamp]
        public byte[] RowVersion { get; private set; } = [];

        public bool ChangeStatus(ClaimStatus newStatus)
        {
            if(Status == ClaimStatus.Reported && newStatus == ClaimStatus.UnderReview)
            {
                Status = newStatus;
                return true;
            }

            if(Status == ClaimStatus.UnderReview && (newStatus == ClaimStatus.Rejected || newStatus == ClaimStatus.Approved))
            {
                Status = newStatus;
                return true;
            }

            if(Status == ClaimStatus.Approved || Status == ClaimStatus.Rejected)
            {
                return false;
            }

            return false;
        }
    }
}
