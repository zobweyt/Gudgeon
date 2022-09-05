using Discord;
using Discord.Interactions;
using System.Collections.Concurrent;

namespace Gudgeon;

public sealed class DoMassUseCheck : PreconditionAttribute
{
    public static readonly ConcurrentDictionary<ulong, string> Limits = new();

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider services)
    {
        string contextId = command.Module.Name + "//" + command.MethodName + "//" + command.Name;
        ulong id = context.Guild?.Id ?? context.User.Id;

        if (Limits.Any(x => x.Key == id && x.Value == contextId))
            return Task.FromResult(PreconditionResult.FromError("The command is already running in this guild."));

        Limits.GetOrAdd(id, contextId);
        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}