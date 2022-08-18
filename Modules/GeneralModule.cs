using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules;

public class GeneralModule : GudgeonModuleBase
{
    [SlashCommand("versions", "Provides inforamtion about the application's versions")]
    public async Task VersionsAsync()
    {
        var bot = Context.Client.CurrentUser;

        var embed = new GudgeonEmbedBuilder()
            .WithThumbnailUrl(bot.GetAvatarUrl() ?? bot.GetDefaultAvatarUrl())
            .AddField($"{Emojis.Folder} Versions", 
            "Gudgeon - **1.4.0**\n" +
            "C# - **10**\n" +
            "Discord.NET - **3.7.2**\n")
            .Build();

        await ModifyOriginalResponseAsync(x => x.Embed = embed);
    }

    [SlashCommand("latency", "Displays bot's latency")]
    public async Task LatencyAsync()
        => await DisplayLatencyAsync();

    [ComponentInteraction("latency_refresh")]
    public async Task LatencyRefreshAsync()
        => await DisplayLatencyAsync();

    private async Task DisplayLatencyAsync()
    {
        var embed = new GudgeonEmbedBuilder()
            .AddField($"{Emojis.PingPong} Latency", $"**{Context.Client.Latency}** ms")
            .Build();

        var button = new ComponentBuilder()
            .WithButton("Refresh", "latency_refresh", ButtonStyle.Secondary)
            .Build();

        void Message(MessageProperties x)
        {
            x.Embed = embed;
            x.Components = button;
        }

        if (Context.Interaction is SocketMessageComponent component) 
            await component.UpdateAsync(Message);
        else
            await ModifyOriginalResponseAsync(Message);
    }
}