using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Gudgeon.Modules;

public class GeneralModule : GudgeonModuleBase
{
    public GeneralModule(InteractiveService interactive)
        : base(interactive)
    {
    }

    [SlashCommand("ping", "Displays bot's latency")]
    public async Task PingAsync()
    {
        var embed = new EmbedBuilder()
            .WithTitle("Pong!")
            .WithDescription($"{Context.Client.Latency} ms")
            .WithColor(Colors.Primary)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("avatar", "Get an user avatar in high quality")]
    public async Task AvatarAsync(IUser? user = null)
    {
        user ??= Context.User;

        var embed = new EmbedBuilder()
            .WithTitle(user.ToString())
            .WithImageUrl(user.GetAvatarUrl(size: 4096) ?? user.GetDefaultAvatarUrl())
            .WithColor(Colors.Primary)
            .Build();

        await RespondAsync(embed: embed);
    }
}