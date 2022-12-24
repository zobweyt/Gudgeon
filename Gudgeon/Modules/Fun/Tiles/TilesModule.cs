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

    [SlashCommand("2048", "The classic 2048 game", runMode: RunMode.Async)]
    public async Task TilesAsync(
        [Summary("size", "The size of the board")][Choice("4x4", 4), Choice("6x6", 6)] int boardSize = 4)
    {
        Player player = new(Context.User);
        TilesCore game = new(player, boardSize);
        BoardConverter converter = new();

        string guide = $"{player.User.Mention}, this is 2048 game in discord.\n" +
                       "Click the buttons for the direction you want to shift the board.\n" +
                       "Tiles with the same value will join together.\n" +
                       "Try to reach the highest 2048 tile!";

        await Context.Interaction.RespondWithBoard(converter.ConvertToString(game, guide));

        while (true)
        {
            IUserMessage? response = await GetOriginalResponseAsync();

            var result = await _interactive.NextMessageComponentAsync(x =>
                x.Message.Id == response.Id &&
                x.User.Id == Context.Interaction.User.Id,
                timeout: TimeSpan.FromMinutes(5));

            if (result.IsTimeout)
            {
                await DeleteOriginalResponseAsync();
                break;                
            }

            Direction direction = CustomIDs.Controls.First(x => x.Value == result.Value.Data.CustomId).Key;
            game.TryMoveBoard(direction);

            if (game.HasMaxTile() || !game.CanShiftBoard())
            {
                string footer = $"{player.User.Mention}, you have ended the game with **{player.Score}** points!";
                await result.Value.RespondWithBoard(converter.ConvertToString(game, footer));
                break;
            }

            await result.Value.RespondWithBoard(converter.ConvertToString(game, $"{player.User.Mention} | score: **{player.Score}**"));
        }
    }
}