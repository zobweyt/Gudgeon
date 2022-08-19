using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Services;

internal sealed partial class InteractionHandler : DiscordClientService
{
    private Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        if (interaction.Type == InteractionType.ApplicationCommand)
            _ = interaction.RespondWithStyleAsync(new ProcessingStyle(), $"The bot is thinking {Emojis.Animated.Loading}");

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
    private Task InteractionExecutedAsync(ICommandInfo command, IInteractionContext context, IResult result)
    {
        if (string.IsNullOrEmpty(result.ErrorReason))
            return Task.CompletedTask;

        EmbedStyle style = result.IsSuccess ? new SuccessStyle() : new ErrorStyle();
        _ = context.Interaction.ModifyWithStyleAsync(style, result.ErrorReason);

        TimeSpan? span = result is GudgeonResult gudgeonResult ? gudgeonResult.DelayedDeleteDuration : null;

        if (!result.IsSuccess)
            span = result.Error != InteractionCommandError.Exception ? span == null ? TimeSpan.FromSeconds(8) : span : null;

        if (span != null)
            _ = context.Interaction.DelayedDeleteResponseAsync(span.Value);

        return Task.CompletedTask;
    }
}