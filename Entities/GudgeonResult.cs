using Discord.Interactions;

namespace Gudgeon;

public class GudgeonResult : RuntimeResult
{
    private GudgeonResult(InteractionCommandError? error, string message, TimeSpan? deletionDelay)
        : base(error, message)
    {
        DeletionDelay = deletionDelay;
    }

    public TimeSpan? DeletionDelay { get; init; } = null;

    public static GudgeonResult FromSuccess(string? message = null, TimeSpan? deletionDelay = null)
        => new(null, message ?? string.Empty, deletionDelay);
    public static GudgeonResult FromError(string message, TimeSpan? deletionDelay = null)
        => new(InteractionCommandError.Unsuccessful, message, deletionDelay);
}