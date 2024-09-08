namespace Gudgeon;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.ViewChannel)]
[RequireBotPermission(ChannelPermission.ReadMessageHistory)]
[RequireBotPermission(ChannelPermission.SendMessages)]
public abstract class GudgeonModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly InteractiveService _interactiveService;

    protected GudgeonModuleBase(InteractiveService interactive)
    {
        _interactiveService = interactive;
    }
}