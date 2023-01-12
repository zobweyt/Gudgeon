using Discord.Interactions;

namespace Gudgeon;

public class GudgeonResult : RuntimeResult
{
    private GudgeonResult(InteractionCommandError? error, string message)
        : base(error, message)
    {
    }

    public static GudgeonResult FromSuccess(string? message = null)
        => new(null, message ?? string.Empty);
    public static GudgeonResult FromError(string message)
        => new(InteractionCommandError.Unsuccessful, message);
}