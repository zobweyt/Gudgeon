using Discord;

namespace Gudgeon.Modules.Fun.Tiles;

public class BoardControlsBuilder : ComponentBuilder
{
    public BoardControlsBuilder()
    {
        WithButton("\0", "tiles_empty_control_left", disabled: true, style: ButtonStyle.Secondary);
        WithButton(emote: Emoji.Parse("⬆️"), customId: CustomIDs.Controls[Direction.Up]);
        WithButton("\0", "tiles_empty_control_right", disabled: true, style: ButtonStyle.Secondary);
        WithButton(emote: Emoji.Parse("⬅️"), customId: CustomIDs.Controls[Direction.Left], row: 1);
        WithButton(emote: Emoji.Parse("⬇️"), customId: CustomIDs.Controls[Direction.Down], row: 1);
        WithButton(emote: Emoji.Parse("➡️"), customId: CustomIDs.Controls[Direction.Right], row: 1);
    }
}
