using Discord;
using Discord.Interactions;
using System.Collections.Concurrent;

namespace Gudgeon;

public sealed class DisableConcurrentExecutionAttribute : PreconditionAttribute
{
    public static readonly ConcurrentDictionary<ulong, string> Items = new();

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        ulong id = context.Guild?.Id ?? context.User.Id;
        string contextId = commandInfo.Module.Name + "//" + commandInfo.MethodName + "//" + commandInfo.Name;

        if (Items.Any(x => x.Key == id && x.Value == contextId))
            return Task.FromResult(PreconditionResult.FromError("The command is already running."));

        Items.GetOrAdd(id, contextId);
        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}