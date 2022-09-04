using Discord;
using Discord.Interactions;
using System.Threading.Channels;

namespace Gudgeon.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
public partial class ModerationModule : GudgeonModuleBase
{
    [RateLimit]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [SlashCommand("clean", "Delete multiple channel messages")]
    public async Task<RuntimeResult> CleanAsync(
        [Summary("amount", $"The number of messages to clean up.")] int amount)
    {
        await DeferAsync();

        //You can get 100 messages each time only
        //var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        //var youngMessages = messages.Where(x => x.Timestamp > DateTime.Now.AddDays(-14)).Skip(1).ToList();
        //await (Context.Channel as ITextChannel).DeleteMessagesAsync(youngMessages);

        int counter = 0;
        var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        do
        {
            var messageIds = messages.Where(a => a != messages.FirstOrDefault() && a.Timestamp > DateTime.UtcNow.AddDays(-6)).Select(a => a.Id);

            await (Context.Channel as ITextChannel).DeleteMessagesAsync(messageIds);

            counter += messages.Count();

            var latestMessage = messages.LastOrDefault();
            if (latestMessage != null)
            {
                if (counter < amount && latestMessage.Timestamp > DateTime.UtcNow.AddDays(-6))
                    messages = await Context.Channel.GetMessagesAsync(latestMessage, Direction.Before, 100).FlattenAsync();
                else
                    messages = null;
            }
        } while (messages != null);

        return CommandResult.FromSuccess($"{counter -1} messages have been successfully cleaned.", TimeSpan.FromSeconds(8));
    }
}