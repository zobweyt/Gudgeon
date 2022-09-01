using Discord.Interactions;

namespace Gudgeon;

public class CommandResult : RuntimeResult
{
    private CommandResult(InteractionCommandError? error, string message, TimeSpan? deletionDelay)
        : base(error, message)
    {
        DeletionDelay = deletionDelay;
    }

    public TimeSpan? DeletionDelay { get; init; } = null;

    public static CommandResult FromSuccess(string? message = null, TimeSpan? deletionDelay = null)
        => new(null, message ?? string.Empty, deletionDelay);
    public static CommandResult FromError(string message, TimeSpan? deletionDelay = null)
        => new(InteractionCommandError.Unsuccessful, message, deletionDelay);
}