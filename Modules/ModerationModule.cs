using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
public class ModerationModule : GudgeonModuleBase
{
    [RequireBotPermission(GuildPermission.BanMembers)]
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

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("timeout", "Timeouts a member")]
    public async Task<RuntimeResult> TimeoutAsync(
        [Summary("user", "The user to be timed out")] SocketGuildUser user,
        [Summary("span", "The span of timeout (30s, 15m, 2d)")] TimeSpan span)
    {
        if (user.IsHierarchyHigher())
            return GudgeonResult.FromError($"{user.Mention} hierarchy is higher than bot's.");
        
        await user.SetTimeOutAsync(span);
        return GudgeonResult.FromSuccess($"{user.Mention} has been timed out until <t:{(DateTimeOffset.Now + span).ToUnixTimeSeconds()}:t>.");
    }
}