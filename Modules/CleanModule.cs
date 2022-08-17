using Discord;
using Discord.WebSocket;
using Discord.Interactions;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
[RequireBotPermission(ChannelPermission.ManageMessages)]
public class CleanModule : GudgeonModuleBase
{
    [SlashCommand("clean", "Deletes multiple channel messages")]
    public async Task<RuntimeResult> CleanAsync(
        [Summary("amount", "The number of messages to clean up between 2 and 500")] int amount)
    {
        if (IsBeyoundLimit(amount))
            return GudgeonResult.FromError($"Can not clean **{amount}** messages, `amount` should be between **2** and **500**.");     

        var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        var youngMessages = messages.Where(x => x.Timestamp > DateTime.Now.AddDays(-14)).Skip(1);
        await CleanMessagesAsync(youngMessages);

        return GudgeonResult.FromSuccess($"**{youngMessages.Count()}** messages been successfully cleaned.", true);
    }
    private async Task CleanMessagesAsync(IEnumerable<IMessage> messages)
    {
        await ModifyWithStyleAsync(new ProcessingStyle(), $"**{messages.Count()}** messages is being processed {Emojis.Animated.Loading}");
        await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
    }
    private bool IsBeyoundLimit(int amount)
    {
        if (amount < 2 || amount > 500)
            return true;
        return false;
    }
}