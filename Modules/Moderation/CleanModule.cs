using Discord;
using Discord.Interactions;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
public partial class ModerationModule : GudgeonModuleBase
{
    public static int MaxCleanAmount { get { return 500; } }
    public static int MinCleanAmount { get { return 2; } }

    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [SlashCommand("clean", "Delete multiple channel messages")]
    public async Task<RuntimeResult> CleanAsync(
        [Summary("amount", $"The number of messages to clean up.")] int amount)
    {
        await Context.Interaction.DeferAsync();

        if (amount < MinCleanAmount || amount > MaxCleanAmount)
            return GudgeonResult.FromError($"Can not clean {amount} messages! Amount should be between {MinCleanAmount} and {MaxCleanAmount}.", TimeSpan.FromSeconds(12));

        var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        var youngMessages = messages.Where(x => x.Timestamp > DateTime.Now.AddDays(-14)).Skip(1).ToList();
        await (Context.Channel as ITextChannel).DeleteMessagesAsync(youngMessages);

        return GudgeonResult.FromSuccess($"{youngMessages.Count} messages have been successfully cleaned.", TimeSpan.FromSeconds(8));
    }
}