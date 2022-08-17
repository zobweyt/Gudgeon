using Discord.Interactions;

namespace Gudgeon.Modules;

public class GeneralModule : GudgeonModuleBase
{
    [SlashCommand("versions", "Provides inforamtion about the application's versions")]
    public async Task VersionsAsync()
    {
        var bot = Context.Client.CurrentUser;

        var embed = new GudgeonEmbedBuilder()
            .WithThumbnailUrl(bot.GetAvatarUrl() ?? bot.GetDefaultAvatarUrl())
            .AddField($"{Emojis.Folder} Versions", "Gudgeon - **1.4.0**\nC# - **10**\nDiscord.NET - **3.7.2**\n")
            .Build();

        await ModifyOriginalResponseAsync(x => x.Embed = embed);
    }

    [SlashCommand("latency", "Displays bot's latency")]
    public async Task LatencyAsync()
    {
        var embed = new GudgeonEmbedBuilder()
            .AddField($"{Emojis.PingPong} Delay", $"**{Context.Client.Latency}** ms")
            .Build();

        await ModifyOriginalResponseAsync(x => x.Embed = embed);
    }
}