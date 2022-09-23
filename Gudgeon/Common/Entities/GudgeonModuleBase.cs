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
    protected readonly InteractiveService _interactive;
    protected GudgeonModuleBase(InteractiveService interactive)
    {
        _interactive = interactive;
    }
}