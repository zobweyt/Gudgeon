﻿using Discord;
using Discord.WebSocket;
using Discord.Interactions;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
[RequireBotPermission(ChannelPermission.ManageMessages)]
public class CleanModule : GudgeonModuleBase
{
    public static int LowerCleanLimit { get { return 2; } }
    public static int UpperCleanLimit { get { return 500; } }

    [SlashCommand("clean", "Deletes multiple channel messages")]
    public async Task<RuntimeResult> CleanAsync(
        [Summary("amount", $"The number of messages to clean up between 2 and 500")] int amount)
    {
        if (IsBeyoundLimit(amount))
            return GudgeonResult.FromError($"Can not clean **{amount}** messages, `amount` should be between **{LowerCleanLimit}** and **{UpperCleanLimit}**.");     

        var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        var youngMessages = messages.Where(x => x.Timestamp > DateTime.Now.AddDays(-14)).Skip(1);
        await CleanMessagesAsync(youngMessages);

        return GudgeonResult.FromSuccess($"**{youngMessages.Count()}** messages been successfully cleaned.", TimeSpan.FromSeconds(8));
    }
    private async Task CleanMessagesAsync(IEnumerable<IMessage> messages)
    {
        await ModifyWithStyleAsync(new ProcessingStyle(), $"**{messages.Count()}** messages is being processed {Emojis.Animated.Loading}");
        if (Context.Channel is SocketTextChannel channel)
            await channel.DeleteMessagesAsync(messages);
    }

    private bool IsBeyoundLimit(int amount)
    {
        if (amount < LowerCleanLimit || amount > UpperCleanLimit)
            return true;
        return false;
    }
}