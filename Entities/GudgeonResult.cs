using Discord.Interactions;

namespace Gudgeon;

public class GudgeonResult : RuntimeResult
{
    private GudgeonResult(InteractionCommandError? error, string description, bool autoDelete)
        : base(error, description)
    {
        AutoDelete = autoDelete;
    }

    public bool AutoDelete { get; }

    public static GudgeonResult FromSuccess(string? description = null, bool autoDelete = false)
        => new(null, description ?? string.Empty, autoDelete);
    public static GudgeonResult FromError(string description, bool autoDelete = true)
        => new(InteractionCommandError.Unsuccessful, description, autoDelete);
}