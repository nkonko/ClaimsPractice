namespace ClaimsWebApi.Responses
{
    public class PolicyClaimsSummary
    {
        public required string PolicyNumber { get; set; }

        public int TotalClaims { get; set; }

        public int ApprovedClaims { get; set; }

        public DateTime LastOccurredAtUtc { get; set; }
    }
}
