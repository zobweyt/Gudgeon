using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules;

public class GeneralModule : GudgeonModuleBase
{
    [SlashCommand("whois", "Get information about guild member")]
    public async Task WhoisAsync(IGuildUser? user = null)
    {
        user ??= Context.User as IGuildUser;

        var embed = new GudgeonEmbedBuilder()
            .WithTitle($"{user.Username}#{user.Discriminator}")
            .AddField("Joined", $"<t:{user.JoinedAt.Value.ToUnixTimeSeconds()}:F>")
            .AddField("Created", $"<t:{user.CreatedAt.ToUnixTimeSeconds()}:F>")
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("avatar", "Get a user avatar")]
    public async Task AvatarAsync(IGuildUser? user = null)
    {
        user ??= Context.User as IGuildUser;

        var embed = new GudgeonEmbedBuilder()
            .WithTitle($"{user.Username}#{user.Discriminator}")
            .WithImageUrl((user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()).Replace("128", "4096"))
            .Build();

        await RespondAsync(embed: embed);
    }
}