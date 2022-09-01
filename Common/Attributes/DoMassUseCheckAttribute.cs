using Discord;
using Discord.Interactions;
using System.Collections.Concurrent;

namespace Gudgeon;

public sealed class DoMassUseCheck : PreconditionAttribute
{
    public static readonly ConcurrentDictionary<ulong, string> LimitedGuilds = new();
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        string contextId = commandInfo.Module + "//" + commandInfo.MethodName + "//" + commandInfo.Name;
        if (LimitedGuilds.Any(x => x.Key == context.Guild.Id && x.Value == contextId))
            return Task.FromResult(PreconditionResult.FromError($"A mass {commandInfo.Name} operation is already in progress."));

        LimitedGuilds.GetOrAdd(context.Guild.Id, contextId);
        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}