using Discord.Interactions;

namespace Gudgeon;

public class GudgeonResult : RuntimeResult
{
    private GudgeonResult(InteractionCommandError? error, string message, bool ephemeral)
        : base(error, message)
    {
        IsEphemeral = ephemeral;
    }

    public bool IsEphemeral { get; init; } = false;

    public static GudgeonResult FromSuccess(string? message = null, bool ephemeral = false)
        => new(null, message ?? string.Empty, ephemeral);
    public static GudgeonResult FromError(string message, bool ephemeral = false)
        => new(InteractionCommandError.Unsuccessful, message, ephemeral);
}