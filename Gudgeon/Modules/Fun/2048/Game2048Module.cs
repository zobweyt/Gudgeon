using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Gudgeon.Modules.Fun._2048;

public class Game2048Module : GameModuleBase
{
    public Game2048Module(InteractiveService interactive)
        : base(interactive)
    {
    }

    [RateLimit(seconds: 32, requests: 1)]
    [SlashCommand("2048", "The classic 2048 game", runMode: RunMode.Async)]
    public async Task RunGameAsync()
    {
        Game2048 game = new(Context.User);

        string guide = "\nClick the buttons for the direction you want to shift the board." +
                         "\nTiles with the same value will join together." +
                         "\nTry to reach the 2048 tile!";
        await RespondAsync(game.GetDisplayBoard() + guide, components: BoardShiftingButtons);

        while (true)
        {
            var (direction, interaction) = await GetDirection();
            if (!game.TryMoveBoard(direction))
            {
                await DisplayBoard(interaction, game.GetDisplayBoard());
                continue;
            }
            if (game.HasMaxTile() | !game.TryGenerateTile())
            {
                await DisplayBoard(interaction, game.GetDisplayBoard());
                break;
            }
            await DisplayBoard(interaction, game.GetDisplayBoard());
        }
    }
    private async Task DisplayBoard(SocketMessageComponent component, string board)
    {
        await component.UpdateAsync(x =>
        {
            x.Content = board;
            x.Components = BoardShiftingButtons;
        });
    }
    private async Task<(Direction, SocketMessageComponent?)> GetDirection()
    {
        IUserMessage? response = await GetOriginalResponseAsync();

        var result = await _interactive.NextMessageComponentAsync(x =>
            x.Message.Id == response.Id &&
            x.User.Id == Context.Interaction.User.Id,
            timeout: TimeSpan.FromMinutes(3));
                
        return (result.Value.Data.CustomId switch
        {
            "2048_move_board_up" => Direction.Up,
            "2048_move_board_down" => Direction.Down,
            "2048_move_board_left" => Direction.Left,
            "2048_move_board_right" => Direction.Right,
            _ => throw new NotImplementedException()
        }, result.Value);
    }
    private MessageComponent BoardShiftingButtons
    {
        get
        {
            return new ComponentBuilder()
            .WithButton(" ", "2048_empty_1", disabled: true, style: ButtonStyle.Secondary)
            .WithButton(emote: Emoji.Parse("⬆️"), customId: "2048_move_board_up")
            .WithButton(" ", "2048_empty_2", disabled: true, style: ButtonStyle.Secondary)
            .WithButton(emote: Emoji.Parse("⬅️"), customId: "2048_move_board_left", row: 1)
            .WithButton(emote: Emoji.Parse("⬇️"), customId: "2048_move_board_down", row: 1)
            .WithButton(emote: Emoji.Parse("➡️"), customId: "2048_move_board_right", row: 1)
            .Build();
        }
    }
}