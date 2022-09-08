using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Gudgeon.Modules.Fun._2048;

public class GameModule : GudgeonModuleBase
{
    public GameModule(InteractiveService interactive)
        : base(interactive)
    {
    }

    [RequireBotPermission(GuildPermission.UseExternalEmojis)]
    [SlashCommand("2048", "The classic 2048 game", runMode: RunMode.Async)]
    public async Task IntroductionAsync()
    {
        Embed embed = new EmbedBuilder()
            .WithTitle("Introduction")
            .WithDescription(
            "Click the buttons for the direction you want to shift the board.\n" +
            "Tiles with the same value will join together.\n" +
            "Try to reach the 2048 tile!")
            .WithFooter($"{Context.User.Username}#{Context.User.Discriminator} | Use button below to start!")
            .WithColor(Colors.Primary)
            .Build();

        MessageComponent builder = new ComponentBuilder()
            .WithButton("Start", "2048_start", ButtonStyle.Success)
            .Build();

        await RespondAsync(embed: embed, components: builder);
        await RunGameAsync();
    }

    private async Task RunGameAsync()
    {
        Game game = new(Context.User);
        await UpdateResponseAsync((await WaitForButtonPress()).Value, game.GetDisplayBoard(), MessageComponents);

        while (true)
        {
            var result = await WaitForButtonPress();
            if (!game.MoveBoard(result.Value.Data.CustomId))
            {
                await UpdateResponseAsync(result.Value, game.GetDisplayBoard(), MessageComponents);
                continue;
            }

            if (game.HasMaxTile())
            {
                game.TryGenerateTile();
                await UpdateResponseAsync(result.Value, game.GetDisplayBoard() + "\nVictory. You won! :tada:");
                break;
            }
            if (!game.TryGenerateTile())
            {
                await UpdateResponseAsync(result.Value, game.GetDisplayBoard() + "\nDefeat. Game over! :skull:");
                break;
            }            

            await UpdateResponseAsync(result.Value, game.GetDisplayBoard(), MessageComponents);
        }
    }

    private async Task UpdateResponseAsync(SocketMessageComponent interaction, string content, MessageComponent? components = null)
    {
        await interaction.UpdateAsync(x =>
        {
            x.Embed = null;
            x.Content = content;
            x.Components = components;
        });
    }
    private async Task<InteractiveResult<SocketMessageComponent?>?> WaitForButtonPress()
    {
        IUserMessage? response = await GetOriginalResponseAsync();

        var result = await _interactive.NextMessageComponentAsync(x =>
            x.Message.Id == response.Id &&
            x.User.Id == Context.Interaction.User.Id,
            timeout: TimeSpan.FromMinutes(5));
        
        if (result.IsTimeout)
        {
            response = await GetOriginalResponseAsync();

            if (response != null)
            {
                await response.DeleteAsync();
            }
        }

        return result;
    }
    private MessageComponent MessageComponents
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