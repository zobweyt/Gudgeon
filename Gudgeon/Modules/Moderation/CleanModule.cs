using Discord;
using Discord.Interactions;

namespace Gudgeon.Modules.Moderation;

public partial class ModerationModule : ModerationModuleBase
{
    [RateLimit]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [SlashCommand("clean", "Delete multiple channel messages")]
    public async Task<RuntimeResult> CleanAsync(
        [Summary("amount", $"The number of messages to clean up.")] [RequireParameterLengthAttribute(2, 100)] int amount)
    {
        await DeferAsync(ephemeral: true);

        var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        var youngMessages = messages.Skip(1).Where(x => x.Timestamp > DateTime.UtcNow.AddDays(-14));
        await (Context.Channel as ITextChannel).DeleteMessagesAsync(youngMessages);

        return GudgeonResult.FromSuccess($"{youngMessages.Count()} messages have been successfully cleaned.");
    }
}