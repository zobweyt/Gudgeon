using Discord;
using Discord.Interactions;

namespace Gudgeon.Modules.Moderation;

[RequireUserPermission(GuildPermission.Administrator)]
public partial class ModerationModule : GudgeonModuleBase
{
    [RequireBotPermission(GuildPermission.Administrator)]
    [SlashCommand("sync", "Sync channel permissions with its category")]
    public async Task<RuntimeResult> SyncAsync(
        [Summary("channel", "The channel to sync")] [ChannelTypes(ChannelType.Text, ChannelType.News, ChannelType.Forum, ChannelType.Voice, ChannelType.Stage)] INestedChannel? channel = null)
    {
        channel ??= Context.Channel as ITextChannel;

        await channel.SyncPermissionsAsync();
        return CommandResult.FromSuccess("Channel permissions have been synced.", TimeSpan.FromSeconds(8));
    }

    [RequireBotPermission(GuildPermission.ManageNicknames)]
    [SlashCommand("nickname", "Change a user nickname")]
    public async Task<RuntimeResult> NicknameAsync(
        [Summary("user", "The user to change nickname")] [DoHierarchyCheck] IGuildUser user, 
        [Summary("nickname", "The nickname to be set")] [RequireParameterLength(32)] string? nickname = null)
    {
        nickname ??= user.Username;

        await user.ModifyAsync(x => x.Nickname = nickname);
        return CommandResult.FromSuccess($"Changed nickname for {user.Mention}.", TimeSpan.FromSeconds(8));
    }
}