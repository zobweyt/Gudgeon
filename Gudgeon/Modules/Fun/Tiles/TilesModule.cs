using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Gudgeon.Modules.Fun.Tiles;

public class TilesModule : GameModuleBase
{
    public TilesModule(InteractiveService interactive)
        : base(interactive)
    {
    }

    [SlashCommand("2048", "Built-in implementation of the classic 2048 game in discord", runMode: RunMode.Async)]
    public async Task TilesAsync([Summary("size", "The size of the board")][Choice("4x4", 4), Choice("6x6", 6)] int maxBoardSize = 4)
    {
        string endReason = await StartGameAsync(maxBoardSize);
        
        IUserMessage? response = await GetOriginalResponseAsync();

        if (response != null)
        {
            await response.ModifyAsync(message => message.Components = new ComponentBuilder().Build());
        }

        await FollowupAsync(endReason);
    }

    public async Task<string> StartGameAsync(int maxBoardSize)
    {
        Player player = new(Context.User);
        TilesCore game = new(player, maxBoardSize);

        await Context.Interaction.RespondAsync(game.ToDiscordMessageContent(), components: new BoardControlsBuilder().Build());
        IUserMessage? response = await GetOriginalResponseAsync();

        while (true)
        {
            var result = await _interactive.NextMessageComponentAsync(interaction =>
                interaction.Message.Id == response.Id &&
                interaction.User.Id == Context.Interaction.User.Id,
                timeout: TimeSpan.FromMinutes(3)
            );

            if (result.IsTimeout)
            {
                return $":hourglass: {player.User.Mention}, timeout! you have been inactive for too long.";
            }

            Direction direction = CustomIDs.Controls.First(x => x.Value == result.Value.Data.CustomId).Key;
            bool shifted = game.TryMoveBoard(direction);

            if (shifted == false)
            {
                await result.Value.RespondAsync($":ping_pong: {player.User.Mention}, it's not possible to shift the board {direction.ToString().ToLower()}!", ephemeral: true);
                continue;
            }

            await result.Value.UpdateAsync(message => message.Content = game.ToDiscordMessageContent());

            if (game.HasMaxTile())
            {
                return $":tada: {player.User.Mention}, congratulations! you have won the game!";
            }

            if (game.CanShiftBoard() == false)
            {
                return $":jigsaw: {player.User.Mention}, you have lost. hope luck will be on your side next time!";
            }
        }
    }
}