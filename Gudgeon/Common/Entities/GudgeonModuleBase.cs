namespace Gudgeon;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.ViewChannel)]
[RequireBotPermission(ChannelPermission.ReadMessageHistory)]
[RequireBotPermission(ChannelPermission.SendMessages)]
public abstract class GudgeonModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly InteractiveService _interactiveService;
    protected readonly GudgeonDbContext _dbContext;

    protected GudgeonModuleBase(InteractiveService interactive, GudgeonDbContext dbContext)
    {
        _interactiveService = interactive;
        _dbContext = dbContext;
    }
}