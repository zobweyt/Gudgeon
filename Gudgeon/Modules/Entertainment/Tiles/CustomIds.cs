using Gudgeon.Entertainment.Tiles;

namespace Gudgeon.Modules.Entertainment;

internal static class CustomIds
{
    public static readonly Dictionary<Control, string> Controls = new()
    {
        [Gudgeon.Entertainment.Tiles.Control.Up] = "tiles_control_up",
        [Gudgeon.Entertainment.Tiles.Control.Down] = "tiles_control_down",
        [Gudgeon.Entertainment.Tiles.Control.Left] = "tiles_control_left",
        [Gudgeon.Entertainment.Tiles.Control.Right] = "tiles_control_right"
    };
}