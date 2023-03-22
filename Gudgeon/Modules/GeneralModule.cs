using Gudgeon.Pagination;

namespace Gudgeon.Modules;

[Group("", "General features")]
public class GeneralModule : GudgeonModuleBase
{
    private readonly InteractionService _interactionService;

    public GeneralModule(InteractiveService interactiveService, GudgeonDbContext dbContext, InteractionService interactionService)
        : base(interactiveService, dbContext)
    {
        _interactionService = interactionService;
    }

    [SlashCommand("ping", "Check the Gudgeon response time.")]
    public async Task PingAsync()
    {
        var embed = new EmbedBuilder()
            .WithTitle("Pong!")
            .WithDescription($"The Gudgeon response time is {Context.Client.Latency} ms.")
            .WithColor(Colors.Primary)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("avatar", "Get a user avatar in high resolution.")]
    public async Task AvatarAsync([Summary(description: "The user to get avatar.")] IUser? user = null)
    {
        user ??= Context.User;

        var embed = new EmbedBuilder()
            .WithTitle(user.ToString())
            .WithImageUrl(user.GetAvatarUrl(size: 4096) ?? user.GetDefaultAvatarUrl())
            .WithColor(Colors.Primary)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("help", "Get detailed information about all the Gudgeon features.", runMode: RunMode.Async)]
    public async Task HelpAsync()
    {
        var paginatorBuilder = new GudgeonStaticPaginatorBuilder().AddUser(Context.User);
        var commands = await Context.Guild.GetApplicationCommandsAsync();
        
        foreach (var module in _interactionService.Modules.Where(x => !x.IsSubModule))
        {
            var pageBuilder = new PageBuilder()
                .WithTitle(module.Description)
                .WithColor(Colors.Primary);

            foreach (var command in module.GetSlashCommandsInSubModules())
                pageBuilder.AddField(command.GetMention(commands), command.Description);
            
            paginatorBuilder.AddPage(pageBuilder);
        }

        await _interactiveService.SendPaginatorAsync(paginatorBuilder.Build(), Context.Interaction);
    }

    [SlashCommand("invite", "Get an invitation link to invite the Gudgeon.")]
    public async Task InviteAsync()
    {
        var embed = new EmbedBuilder()
            .WithTitle("Gudgeon invitation")
            .WithDescription("Click on the button below to continue")
            .WithColor(Colors.Primary)
            .Build();

        var components = new ComponentBuilder()
            .WithButton("Authorize", style: ButtonStyle.Link, url: Context.Client.GetBotInviteUrl())
            .Build();
        
        await RespondAsync(embed: embed, components: components);
    }
}