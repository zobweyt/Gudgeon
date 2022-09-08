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

    [SlashCommand("whois", "Get information about guild member")]
    public async Task WhoisAsync(IGuildUser? user = null)
    {
        user ??= Context.User as IGuildUser;

        var embed = new EmbedBuilder()
            .WithTitle($"{user.Username}#{user.Discriminator}")
            .AddField("Joined", $"<t:{user.JoinedAt.Value.ToUnixTimeSeconds()}:R>")
            .AddField("Created", $"<t:{user.CreatedAt.ToUnixTimeSeconds()}:R>")
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithColor(Colors.Primary)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("avatar", "Get a user avatar")]
    public async Task AvatarAsync(IUser? user = null)
    {
        user ??= Context.User;

        var embed = new EmbedBuilder()
            .WithTitle($"{user.Username}#{user.Discriminator}")
            .WithImageUrl(user.GetAvatarUrl(size: 4096) ?? user.GetDefaultAvatarUrl())
            .WithColor(Colors.Primary)
            .Build();

        await RespondAsync(embed: embed);
    }
}