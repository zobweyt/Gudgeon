using Discord;
using Discord.Interactions;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
public partial class ModerationModule : GudgeonModuleBase
{
    [RateLimit]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [SlashCommand("clean", "Delete multiple channel messages")]
    public async Task<RuntimeResult> CleanAsync(
        [Summary("amount", $"The number of messages to clean up.")] [RequireParameterLength(2, 500)] int amount)
    {
        await DeferAsync();

        var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        var youngMessages = messages.Where(x => x.Timestamp > DateTime.Now.AddDays(-14)).Skip(1).ToList();
        await (Context.Channel as ITextChannel).DeleteMessagesAsync(youngMessages);

        return CommandResult.FromSuccess($"{youngMessages.Count} messages have been successfully cleaned.", TimeSpan.FromSeconds(8));
    }
}