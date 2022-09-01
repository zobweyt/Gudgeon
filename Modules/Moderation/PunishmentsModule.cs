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
        await Context.Guild.AddBanAsync(user, reason: reason);
        return CommandResult.FromSuccess($"{user.Username}#{user.Discriminator} has been banned.");
    }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("unban", "Unban a user")]
    public async Task<RuntimeResult> UnbanAsync(
        [Summary("user", "The user to unban")] IUser user)
    {
        await Context.Guild.RemoveBanAsync(user);
        return CommandResult.FromSuccess($"{user.Username}#{user.Discriminator} has been unbanned.", TimeSpan.FromSeconds(8));
    }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("kick", "Kick a user")]
    public async Task<RuntimeResult> KickAsync(
        [Summary("user", "The user to kick")] [DoHierarchyCheck] IGuildUser user,
        [Summary("reason", "The kick reason")] [RequireParameterLength(512)] string? reason = null)
    {
        await user.KickAsync(reason);
        return CommandResult.FromSuccess($"{user.Username}#{user.Discriminator} has been kicked.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("timeout", "Timeout a user")]
    public async Task<RuntimeResult> TimeoutAsync(
        [Summary("user", "The user to timeout")] [DoHierarchyCheck] IGuildUser user,
        [Summary("span", "The span of timeout (ex. 12m, 32s)")] TimeSpan span)
    {
        await user.SetTimeOutAsync(span);
        return CommandResult.FromSuccess($"{user.Username}#{user.Discriminator} has been timed out until <t:{(DateTimeOffset.Now + span).ToUnixTimeSeconds()}:f>.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("remove-timeout", "Remove a user timeout")]
    public async Task<RuntimeResult> RemoveTimeoutAsync(
        [Summary("user", "The user to remove timeout")] [DoHierarchyCheck] IGuildUser user)
    {
        await user.RemoveTimeOutAsync();
        return CommandResult.FromSuccess($"The timeout for {user.Username}#{user.Discriminator} has been removed.", TimeSpan.FromSeconds(8));
    }
}