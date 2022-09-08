using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Gudgeon.Modules.Moderation;

public partial class ModerationModule : ModerationModuleBase
{
    public ModerationModule(InteractiveService interactive)
        : base(interactive)
    {
    }

    [RequireBotPermission(GuildPermission.Administrator)]
    [SlashCommand("sync", "Sync channel permissions with its category")]
    public async Task<RuntimeResult> SyncAsync(
        [Summary("channel", "The channel to sync")] [ChannelTypes(ChannelType.Text, ChannelType.News, ChannelType.Forum, ChannelType.Voice, ChannelType.Stage)] INestedChannel? channel = null)
    {
        channel ??= Context.Channel as INestedChannel;

        await channel.SyncPermissionsAsync();
        return GudgeonResult.FromSuccess("Channel permissions have been synced.", true);
    }

    [RequireBotPermission(GuildPermission.ManageNicknames)]
    [SlashCommand("nickname", "Change a user nickname")]
    public async Task<RuntimeResult> NicknameAsync(
        [Summary("user", "The user to change nickname")] [DoHierarchyCheck] IGuildUser user, 
        [Summary("nickname", "The nickname to be set")] [RequireParameterLength(32)] string? nickname = null)
    {
        nickname ??= user.Username;

        await user.ModifyAsync(x => x.Nickname = nickname);
        return GudgeonResult.FromSuccess($"Changed nickname for {user.Mention}.");
    }
}