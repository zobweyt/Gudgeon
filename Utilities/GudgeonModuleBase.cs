using Discord;
using Discord.Interactions;

namespace Gudgeon;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.ViewChannel)]
[RequireBotPermission(ChannelPermission.ReadMessageHistory)]
[RequireBotPermission(ChannelPermission.SendMessages)]
public abstract class GudgeonModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    protected virtual async Task RespondWithStyleAsync(EmbedStyle style, string message)
        => await Context.Interaction.RespondWithStyleAsync(style, message);
    protected virtual async Task ModifyWithStyleAsync(EmbedStyle style, string message)
        => await Context.Interaction.ModifyWithStyleAsync(style, message);
    protected virtual async Task DelayedDeleteResponseAsync(TimeSpan span)
        => await Context.Interaction.DelayedDeleteResponseAsync(span);
}