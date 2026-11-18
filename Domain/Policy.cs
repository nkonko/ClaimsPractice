namespace ClaimsWebApi.Domain
{
    public class Policy
    {
        public Guid Id { get; set; }

        public required string PolicyNumber { get; set; }

        public Guid CustomerId { get; set; }
    }
}
