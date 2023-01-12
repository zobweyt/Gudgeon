using Discord;
using Discord.Interactions;
using Fergun.Interactive;
using Gudgeon.Common.Styles;

namespace Gudgeon.Modules.Moderation;

[RequireUserPermission(GuildPermission.Administrator)]
public class ModerationModule : GudgeonModuleBase
{
    public ModerationModule(InteractiveService interactive)
        : base(interactive)
    {
    }

    [RateLimit(seconds: 4, requests: 1)]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [SlashCommand("clean", "Delete multiple channel messages")]
    public async Task<RuntimeResult> CleanAsync(
        [Summary("amount", $"The number of messages to clean up.")][MinValue(2), MaxValue(100)] int amount)
    {
        await DeferAsync(ephemeral: true);

        var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        var youngMessages = messages.Skip(1).Where(x => x.Timestamp > DateTime.UtcNow.AddDays(-14));
        await (Context.Channel as ITextChannel).DeleteMessagesAsync(youngMessages);

        return GudgeonResult.FromSuccess($"{youngMessages.Count()} messages have been successfully cleaned.");
    }

    [RequireBotPermission(GuildPermission.Administrator)]
    [SlashCommand("sync", "Sync channel permissions with its category")]
    public async Task<RuntimeResult> SyncAsync(
        [Summary("channel", "The channel to sync")][ChannelTypes(ChannelType.Text, ChannelType.News, ChannelType.Forum, ChannelType.Voice, ChannelType.Stage)] INestedChannel? channel = null)
    {
        channel ??= Context.Channel as INestedChannel;

        await channel.SyncPermissionsAsync();
        return GudgeonResult.FromSuccess("Channel permissions have been synced.");
    }

    [RequireBotPermission(GuildPermission.ManageNicknames)]
    [SlashCommand("nickname", "Change a user nickname")]
    public async Task<RuntimeResult> NicknameAsync(
        [Summary("user", "The user to change nickname")][DoHierarchyCheck] IGuildUser user, 
        [Summary("nickname", "The nickname to be set")][MaxLength(32)] string? nickname = null)
    {
        nickname ??= user.Username;

        await user.ModifyAsync(x => x.Nickname = nickname);
        return GudgeonResult.FromSuccess($"Changed nickname for {user.Mention}.");
    }

    [RateLimit(seconds: 4, requests: 1)]
    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("ban", "Ban a user")]
    public async Task<RuntimeResult> BanAsync(
        [Summary("user", "The user to ban")][DoHierarchyCheck] IUser user,
        [Summary("reason", "The ban reason")][MaxLength(512)] string? reason = null,
        [Summary("issue", "Create a button with url to provide additional info")] string? issueUrl = null)
    {
        if ((await Context.Guild.GetBanAsync(user)) != null)
            return GudgeonResult.FromError($"{user} have been already banned.");

        var embed = new EmbedBuilder()
            .WithStyle(new SuccessStyle(), $"{user} has been banned permanently")
            .WithDescription(reason != null ? $"**Reason:** {reason}." : null)
            .Build();

        MessageComponent? components = null;

        try
        {
            if (issueUrl != null)
            {
                components = new ComponentBuilder()
                    .WithButton("Issue", style: ButtonStyle.Link, url: issueUrl)
                    .Build();
            }
        }
        catch (InvalidOperationException exception)
        {
            return GudgeonResult.FromError(exception.Message);
        }

        await Context.Guild.AddBanAsync(user, reason: reason);
        await RespondAsync(embed: embed, components: components);
        return GudgeonResult.FromSuccess();
    }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("unban", "Unban a user")]
    public async Task<RuntimeResult> UnbanAsync(
        [Summary("user", "The user to unban")] IUser user)
    {
        if ((await Context.Guild.GetBanAsync(user)) == null)
            return GudgeonResult.FromError($"{user.Mention} have not been banned.");

        await Context.Guild.RemoveBanAsync(user);
        return GudgeonResult.FromSuccess($"{user} has been unbanned.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("kick", "Kick a user")]
    public async Task<RuntimeResult> KickAsync(
        [Summary("user", "The user to kick")][DoHierarchyCheck] IGuildUser user,
        [Summary("reason", "The kick reason")][MaxLength(512)] string? reason = null)
    {
        await user.KickAsync(reason);
        return GudgeonResult.FromSuccess($"{user} has been kicked.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("mute", "Timeout a user")]
    public async Task<RuntimeResult> MuteAsync(
        [Summary("user", "The user to timeout")][DoHierarchyCheck] IGuildUser user,
        [Summary("span", "The span of timeout (ex. 12m, 32s)")] TimeSpan span)
    {
        if (user.TimedOutUntil != null && user.TimedOutUntil.Value >= DateTime.Now + span)
            return GudgeonResult.FromError($"{user.Mention} has been already timed out until <t:{(user.TimedOutUntil.Value).ToUnixTimeSeconds()}:f>.");

        await user.SetTimeOutAsync(span);
        return GudgeonResult.FromSuccess($"{user.Mention} has been timed out until <t:{(DateTimeOffset.Now + span).ToUnixTimeSeconds()}:f>.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("unmute", "Remove a user timeout")]
    public async Task<RuntimeResult> UnmuteAsync(
        [Summary("user", "The user to remove timeout")][DoHierarchyCheck] IGuildUser user)
    {
        if (user.TimedOutUntil == null)
            return GudgeonResult.FromError($"{user.Mention} have not been timed out.");

        await user.RemoveTimeOutAsync();
        return GudgeonResult.FromSuccess($"The timeout for {user.Mention} has been removed.");
    }
}