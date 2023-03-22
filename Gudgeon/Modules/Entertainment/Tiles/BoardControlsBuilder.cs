using Gudgeon.Entertainment.Tiles;

namespace Gudgeon.Modules.Entertainment;

public class BoardControlsBuilder : ComponentBuilder
{
    public BoardControlsBuilder()
    {
        WithButton("\0", "tiles_empty_control_left", disabled: true, style: ButtonStyle.Secondary);
        WithButton(emote: Emoji.Parse("⬆️"), customId: CustomIds.Controls[Control.Up]);
        WithButton("\0", "tiles_empty_control_right", disabled: true, style: ButtonStyle.Secondary);
        WithButton(emote: Emoji.Parse("⬅️"), customId: CustomIds.Controls[Control.Left], row: 1);
        WithButton(emote: Emoji.Parse("⬇️"), customId: CustomIds.Controls[Control.Down], row: 1);
        WithButton(emote: Emoji.Parse("➡️"), customId: CustomIds.Controls[Control.Right], row: 1);
    }
}
