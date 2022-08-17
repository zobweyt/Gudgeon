using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Services;

internal sealed partial class InteractionHandler : DiscordClientService
{
    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(Client, interaction);
            await _service.ExecuteCommandAsync(context, _provider);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Exception occurred whilst attempting to handle interaction.");

            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                var message = await interaction.GetOriginalResponseAsync();
                await message.DeleteAsync();
            }
        }
    }
    private async Task InteractionExecutedAsync(ICommandInfo command, IInteractionContext context, IResult result)
    {
        if (result.ErrorReason == null)
            return;

        bool autoDelete = true;
        if (result is GudgeonResult gudgeonResult)
            autoDelete = gudgeonResult.AutoDelete;

        await context.Interaction.ModifyWithStyleAsync(result.IsSuccess ? new SuccessStyle() : new ErrorStyle(), result.ErrorReason, autoDelete);
    }
}