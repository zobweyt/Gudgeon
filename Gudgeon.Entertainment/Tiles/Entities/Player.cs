using Discord;

namespace Gudgeon.Entertainment.Tiles;

public class Player
{
    public IUser User { get; init; }
    public int Score { get; set; } = 0;

    public Player(IUser user)
    {
        User = user;
    }
}