namespace Gudgeon.Modules.Entertainment;

[Group("", "Games and entertainment")]
[RequireBotPermission(ChannelPermission.UseExternalEmojis)]
public partial class EntertainmentModule : GudgeonModuleBase
{
    protected readonly Random _random = new();

    public EntertainmentModule(InteractiveService interactiveService) 
        : base(interactiveService)
    {
    }

    [SlashCommand("dice", "Roll the dice.")]
    public async Task RandomAsync()
    {
        await RespondAsync($":game_die: {_random.Next(1, 7)}");
    }
}