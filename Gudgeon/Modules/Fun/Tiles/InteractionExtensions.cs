using Discord;
using Discord.WebSocket;

namespace Gudgeon.Modules.Fun.Tiles;

public static class InteractionExtensions
{
    public static async Task<SocketInteraction> RespondWithBoard(this SocketInteraction interaction, string board)
    {
        MessageComponent components = new ComponentBuilder()
            .WithButton(" ", "tiles_empty_control_left", disabled: true, style: ButtonStyle.Secondary)
            .WithButton(emote: Emoji.Parse("⬆️"), customId: CustomIDs.Controls[Direction.Up])
            .WithButton(" ", "tiles_empty_control_right", disabled: true, style: ButtonStyle.Secondary)
            .WithButton(emote: Emoji.Parse("⬅️"), customId: CustomIDs.Controls[Direction.Left], row: 1)
            .WithButton(emote: Emoji.Parse("⬇️"), customId: CustomIDs.Controls[Direction.Down], row: 1)
            .WithButton(emote: Emoji.Parse("➡️"), customId: CustomIDs.Controls[Direction.Right], row: 1)
            .Build();

        if (interaction is SocketMessageComponent component)
        {
            await component.UpdateAsync(x => 
            {
                x.Content = board;
                x.Components = components;
            });
        }
        else
        {
            await interaction.RespondAsync(board, components: components);
        }

        return interaction;
    }
}