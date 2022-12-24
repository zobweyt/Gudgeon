namespace Gudgeon.Modules.Fun.Tiles;

internal static class CustomIDs
{
    public static readonly Dictionary<Direction, string> Controls = new()
    {
        [Direction.Up] = "tiles_control_up",
        [Direction.Down] = "tiles_control_down",
        [Direction.Left] = "tiles_control_left",
        [Direction.Right] = "tiles_control_right"
    };
}