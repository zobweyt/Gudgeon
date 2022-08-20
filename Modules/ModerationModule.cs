using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
public class ModerationModule : GudgeonModuleBase
{
    public static int MaxBanReasonLenght { get { return 512; } }

    [RequireBotPermission(GuildPermission.BanMembers)]
    [SlashCommand("ban", "Bans a user")]
    public async Task<RuntimeResult> BanAsync(
        [Summary("user", "The user to be banned")] IUser target,
        [Summary("reason", "The reason of banning")] string? reason = null)
    {
        if (target is SocketGuildUser guildUser && guildUser.HasHigherHierarchy(true, Context.Interaction))
            return GudgeonResult.FromSuccess();

        if (reason?.Length > MaxBanReasonLenght)
            return GudgeonResult.FromError($"Ban reason length should be less or equal **{MaxBanReasonLenght}**.");

        await Context.Guild.AddBanAsync(target, reason: reason);
        return GudgeonResult.FromSuccess($"{target.Username}#{target.Discriminator} has been banned.");
    }

    [RequireBotPermission(GuildPermission.ModerateMembers)]
    [SlashCommand("timeout", "Timeouts a guild user")]
    public async Task<RuntimeResult> TimeoutAsync(
        [Summary("user", "The user to be timed out")] IGuildUser user,
        [Summary("span", "The span of timeout (ex. 12m, 32s)")] TimeSpan span)
    {
        if (user.HasHigherHierarchy(true, Context.Interaction))
            return GudgeonResult.FromSuccess();
        
        await user.SetTimeOutAsync(span);
        return GudgeonResult.FromSuccess($"{user.Username}#{user.Discriminator} has been timed out until <t:{(DateTimeOffset.Now + span).ToUnixTimeSeconds()}:t>.");
    }
}