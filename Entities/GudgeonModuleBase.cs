using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Gudgeon;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.ViewChannel)]
[RequireBotPermission(ChannelPermission.ReadMessageHistory)]
[RequireBotPermission(ChannelPermission.SendMessages)]
public abstract class GudgeonModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    public override async void BeforeExecute(ICommandInfo command)
        => await Context.Interaction.RespondWithStyleAsync(new ProcessingStyle(), $"The bot is thinking {Emojis.Animated.Loading}");

    protected virtual async Task ModifyWithStyleAsync(EmbedStyle style, string message, bool autoDelete = false)
        => await Context.Interaction.ModifyWithStyleAsync(style, message, autoDelete);
}