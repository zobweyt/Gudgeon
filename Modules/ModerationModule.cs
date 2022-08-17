using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules;

[RequireBotPermission(GuildPermission.BanMembers)]
[RequireUserPermission(GuildPermission.Administrator)]
public class ModerationModule : GudgeonModuleBase
{
    [SlashCommand("ban", "Bans a member")]
    public async Task<RuntimeResult> BanAsync(
        [Summary("user", "The user to be banned")] SocketGuildUser user,
        [Summary("reason", "The reason of banning")] string? reason = null)
    {
        if (user.IsHierarchyHigher())
            return GudgeonResult.FromError($"{user.Mention} hierarchy is higher than bot's.");

        await user.BanAsync(reason: reason);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been banned.");
    }

    [SlashCommand("timeout", "Timeouts a member")]
    public async Task<RuntimeResult> TimeoutAsync(
        [Summary("user", "The user to be timed out")] SocketGuildUser user,
        [Summary("time", "The time of timeout (30s, 15m, 2d)")] TimeSpan time)
    {
        if (user.IsHierarchyHigher())
            return GudgeonResult.FromError($"{user.Mention} hierarchy is higher than bot's.");

        await user.SetTimeOutAsync(time);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been timed out.");
    }
}