using Discord;
using Discord.Interactions;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
public partial class ModerationModule : GudgeonModuleBase
{
    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("ban", "Ban a user")]
    public async Task<RuntimeResult> BanAsync(
        [Summary("user", "The user to ban")] [DoHierarchyCheck] IUser user,
        [Summary("reason", "The ban reason")] [RequireParameterLength(512)] string? reason = null)
    {
        var ban = await Context.Guild.GetBanAsync(user);
        if (ban != null)
            return GudgeonResult.FromError($"{user.Username}#{user.Discriminator} have been already banned.");

        await Context.Guild.AddBanAsync(user, reason: reason);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been banned.");
    }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("unban", "Unban a user")]
    public async Task<RuntimeResult> UnbanAsync(
        [Summary("user", "The user to unban")] IUser user)
    {
        var ban = await Context.Guild.GetBanAsync(user);
        if (ban == null)
            return GudgeonResult.FromError($"{user.Username}#{user.Discriminator} have not been banned.");

        await Context.Guild.RemoveBanAsync(user);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been unbanned.");
    }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("kick", "Kick a user")]
    public async Task<RuntimeResult> KickAsync(
        [Summary("user", "The user to kick")] [DoHierarchyCheck] IGuildUser user,
        [Summary("reason", "The kick reason")] [RequireParameterLength(512)] string? reason = null)
    {
        await user.KickAsync(reason);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been kicked.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("mute", "Timeout a user")]
    public async Task<RuntimeResult> MuteAsync(
        [Summary("user", "The user to timeout")] [DoHierarchyCheck] IGuildUser user,
        [Summary("span", "The span of timeout (ex. 12m, 32s)")] TimeSpan span)
    {
        if (user.TimedOutUntil != null && user.TimedOutUntil.Value >= DateTime.Now + span)
            return GudgeonResult.FromError($"{user.Username}#{user.Discriminator} has been already timed out until <t:{(user.TimedOutUntil.Value).ToUnixTimeSeconds()}:f>.");

        await user.SetTimeOutAsync(span);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been timed out until <t:{(DateTimeOffset.Now + span).ToUnixTimeSeconds()}:f>.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("unmute", "Remove a user timeout")]
    public async Task<RuntimeResult> UnmuteAsync(
        [Summary("user", "The user to remove timeout")] [DoHierarchyCheck] IGuildUser user)
    {
        if (user.TimedOutUntil == null)
            return GudgeonResult.FromError($"{user.Username}#{user.Discriminator} have not been timed out.");

        await user.RemoveTimeOutAsync();
        return GudgeonResult.FromSuccess($"The timeout for {user.Username}#{user.Discriminator} has been removed.");
    }
}