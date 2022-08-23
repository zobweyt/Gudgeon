using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
public partial class ModerationModule : GudgeonModuleBase
{
    public static int MaxReasonLenght { get { return 512; } }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("ban", "Ban a user")]
    public async Task<RuntimeResult> BanAsync(
        [Summary("user", "The user to ban")] SocketUser user,
        [Summary("reason", "The ban reason")] string? reason = null)
    {
        if (user is SocketGuildUser guildUser && guildUser.HasHigherHierarchy(Context.Guild.CurrentUser))
            return GudgeonResult.FromError($"Unable to ban {user.Mention} due to its hierarchy.", TimeSpan.FromSeconds(10));

        if (reason?.Length > MaxReasonLenght)
            return GudgeonResult.FromError($"Ban reason length should be less or equal **{MaxReasonLenght}**.", TimeSpan.FromSeconds(12));

        await Context.Guild.AddBanAsync(user, reason: reason);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been banned.");
    }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("unban", "Unban a user")]
    public async Task<RuntimeResult> UnbanAsync(
        [Summary("user", "The user to unban")] IUser user)
    {
        await Context.Guild.RemoveBanAsync(user);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been unbanned.", TimeSpan.FromSeconds(8));
    }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("kick", "Kick a user")]
    public async Task<RuntimeResult> KickAsync(
        [Summary("user", "The user to kick")] IGuildUser user,
        [Summary("reason", "The kick reason")] string? reason = null)
    {
        if (user.HasHigherHierarchy(Context.Guild.CurrentUser))
            return GudgeonResult.FromError($"Unable to kick {user.Mention} due to its hierarchy.", TimeSpan.FromSeconds(10));

        if (reason?.Length > MaxReasonLenght)
            return GudgeonResult.FromError($"Kick reason length should be less or equal **{MaxReasonLenght}**.", TimeSpan.FromSeconds(12));

        await user.KickAsync(reason);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been kicked.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("timeout", "Timeout a user")]
    public async Task<RuntimeResult> TimeoutAsync(
        [Summary("user", "The user to timeout")] IGuildUser user,
        [Summary("span", "The span of timeout (ex. 12m, 32s)")] TimeSpan span)
    {
        if (user is IGuildUser guildUser && guildUser.HasHigherHierarchy(Context.Guild.CurrentUser))
            return GudgeonResult.FromError($"Unable to timeout {user.Mention} due to its hierarchy.", TimeSpan.FromSeconds(10));

        await user.SetTimeOutAsync(span);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been timed out until <t:{(DateTimeOffset.Now + span).ToUnixTimeSeconds()}:f>.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("remove-timeout", "Remove a user timeout")]
    public async Task<RuntimeResult> RemoveTimeoutAsync(
        [Summary("user", "The user to remove timeout")] IGuildUser user)
    {
        if (user is IGuildUser guildUser && guildUser.HasHigherHierarchy(Context.Guild.CurrentUser))
            return GudgeonResult.FromError($"Unable to remove timeout from {user.Mention} due to its hierarchy.", TimeSpan.FromSeconds(10));

        await user.RemoveTimeOutAsync();
        return GudgeonResult.FromSuccess($"The timeout for {user.Username}#{user.Discriminator} has been removed.", TimeSpan.FromSeconds(8));
    }
}