namespace Gudgeon;

public class RateLimitItem
{
    public string? ContextId { get; init; }
    public DateTime ExpireAt { get; init; }
    public RateLimitItem(string? contextId, DateTime expireAt)
    {
        ContextId = contextId;
        ExpireAt = expireAt;
    }
}