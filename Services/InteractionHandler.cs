using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Services;

internal sealed partial class InteractionHandler : DiscordClientService
{
    private async Task InteractionCreated(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(Client, interaction);
            await _service.ExecuteCommandAsync(context, _provider);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Exception occurred whilst attempting to handle interaction.");
        }
    }
    private async Task InteractionExecuted(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (string.IsNullOrEmpty(result.ErrorReason))
            return;

        EmbedStyle style = result.IsSuccess ? new SuccessStyle() : new ErrorStyle();

        if (context.Interaction.HasResponded)
            await context.Interaction.ModifyWithStyleAsync(style, result.ErrorReason).ConfigureAwait(false);
        else
            await context.Interaction.RespondWithStyleAsync(style, result.ErrorReason).ConfigureAwait(false);

        TimeSpan? span = result is CommandResult gudgeonResult ? gudgeonResult.DeletionDelay : TimeSpan.FromSeconds(8);
        if (span != null)
            await context.Interaction.DelayedDeleteResponseAsync(span.Value).ConfigureAwait(false);

        if (context.User is IGuildUser)
            RemoveGuildLimits(context.Guild.Id, commandInfo);
    }

    private void RemoveGuildLimits(ulong guildId, ICommandInfo commandInfo)
    {
        string? contextId = commandInfo.Module + "//" + commandInfo.MethodName + "//" + commandInfo.Name;
        if (DoMassUseCheck.LimitedGuilds.Any(x => x.Key == guildId && x.Value == contextId))
            DoMassUseCheck.LimitedGuilds.Remove(guildId, out contextId);
    }
}