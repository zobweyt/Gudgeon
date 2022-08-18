using Discord.Interactions;

namespace Gudgeon;

public class GudgeonResult : RuntimeResult
{
    private GudgeonResult(InteractionCommandError? error, string message, TimeSpan? delayedDeleteDuration)
        : base(error, message)
    {
        DelayedDeleteDuration = delayedDeleteDuration;
    }

    public TimeSpan? DelayedDeleteDuration { get; }

    public static GudgeonResult FromSuccess(string? message = null, TimeSpan? delayedDeleteDuration = null)
        => new(null, message ?? string.Empty, delayedDeleteDuration);
    public static GudgeonResult FromError(string message, TimeSpan? delayedDeleteDuration = null)
        => new(InteractionCommandError.Unsuccessful, message, delayedDeleteDuration);
}