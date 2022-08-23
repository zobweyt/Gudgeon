using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Services;

internal sealed partial class InteractionHandler : DiscordClientService
{
    private Task InteractionCreated(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(Client, interaction);
            _ = _service.ExecuteCommandAsync(context, _provider);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Exception occurred whilst attempting to handle interaction.");
        }

        return Task.CompletedTask;
    }
    private Task InteractionExecuted(ICommandInfo command, IInteractionContext context, IResult result)
    {
        if (string.IsNullOrEmpty(result.ErrorReason))
            return Task.CompletedTask;

        EmbedStyle style = result.IsSuccess ? new SuccessStyle() : new ErrorStyle();

        if (context.Interaction.HasResponded)
            _ = context.Interaction.ModifyWithStyleAsync(style, result.ErrorReason);
        else
            _ = context.Interaction.RespondWithStyleAsync(style, result.ErrorReason);

        TimeSpan? span = result is GudgeonResult gudgeonResult ? gudgeonResult.DeletionDelay : TimeSpan.FromSeconds(8);
        if (span != null)
            _ = context.Interaction.DelayedDeleteResponseAsync(span.Value);

        return Task.CompletedTask;
    }
}