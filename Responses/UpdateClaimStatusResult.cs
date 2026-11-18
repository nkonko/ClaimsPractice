namespace ClaimsWebApi.Result
{
    public enum UpdateClaimStatusResult
    {
        Success,
        NotFound,
        InvalidTransition,
        ConcurrencyConflict
    }
}
