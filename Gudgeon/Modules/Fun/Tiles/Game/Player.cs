using Discord;

namespace Gudgeon.Modules.Fun.Tiles;

class Player
{
    public IUser User { get; init; }
    public int Score { get; set; } = 0;

    public Player(IUser user)
    {
        User = user;
    }
}