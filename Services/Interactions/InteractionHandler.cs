using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using Gudgeon.Common.Styles;

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
        }
    }
    private async Task InteractionExecutedAsync(ICommandInfo command, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess && result.Error == InteractionCommandError.UnknownCommand || result.Error == InteractionCommandError.Exception)
            return;

        if (!string.IsNullOrEmpty(result.ErrorReason))
            await HandleInterationAsync(context.Interaction, result);
        
        var contextId = command.Module.Name + "//" + command.MethodName + "//" + command.Name;
        ulong id = context.Guild?.Id ?? context.User.Id;
        if (DoMassUseCheck.Limits.Any(x => x.Key == id && x.Value == contextId))
            DoMassUseCheck.Limits.Remove(id, out contextId);
    }

    private async Task HandleInterationAsync(IDiscordInteraction interaction, IResult result)
    {
        EmbedStyle style = result.IsSuccess ? new SuccessStyle() : new ErrorStyle();
        var embed = new EmbedBuilder()
            .WithStyle(style)
            .WithDescription(result.ErrorReason)
            .Build();

        bool ephemeral = result is not GudgeonResult gudgeonResult || gudgeonResult.IsEphemeral;

        if (interaction.HasResponded)
        {
            await interaction.FollowupAsync(embed: embed, ephemeral: ephemeral);
            return;
        }
        await interaction.RespondAsync(embed: embed, ephemeral: ephemeral);
    }
}