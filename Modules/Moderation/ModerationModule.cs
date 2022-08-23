using Discord;
using Discord.Interactions;

namespace Gudgeon.Modules.Moderation;

[RequireUserPermission(GuildPermission.Administrator)]
public partial class ModerationModule : GudgeonModuleBase
{
    [RequireBotPermission(GuildPermission.Administrator)]
    [SlashCommand("sync", "Sync channel permissions with its category")]
    public async Task<RuntimeResult> SyncAsync(ITextChannel? channel = null)
    {
        channel ??= Context.Channel as ITextChannel;

        if (!channel.CanAccess(Context.Guild.CurrentUser))
            return GudgeonResult.FromError($"Unable to access {channel.Mention} due to its permissions.", TimeSpan.FromSeconds(10));

        await (Context.Channel as ITextChannel).SyncPermissionsAsync();
        return GudgeonResult.FromSuccess("Channel permissions have been synced.", TimeSpan.FromSeconds(8));
    }

    [RequireBotPermission(GuildPermission.ManageNicknames)]
    [SlashCommand("nickname", "Change a user nickname")]
    public async Task<RuntimeResult> NicknameAsync(
        [Summary("user", "The user to change nickname")] IGuildUser user, 
        [Summary("nickname", "The nickname to be set")] string? nickname = null)
    {
        if (user.HasHigherHierarchy(Context.Guild.CurrentUser)
            || user.HasHigherHierarchy(Context.User as IGuildUser))
            return GudgeonResult.FromError($"Unable to change nickname for {user.Mention} due to its hierarchy.", TimeSpan.FromSeconds(10));

        nickname ??= user.Username;

        if (nickname.Length > 32)
            return GudgeonResult.FromError("Nickname should be less or equal 32.", TimeSpan.FromSeconds(12));
        
        await user.ModifyAsync(x => x.Nickname = nickname);
        return GudgeonResult.FromSuccess($"Changed nickname for {user.Mention}.", TimeSpan.FromSeconds(8));
    }
}