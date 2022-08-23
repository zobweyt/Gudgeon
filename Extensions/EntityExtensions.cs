using Discord;
using Discord.WebSocket;

namespace Gudgeon;

internal static class EntityExtensions
{
    public static bool HasHigherHierarchy(this IRole role, IGuildUser user)
    {
        if (role.IsManaged
            || role.Position >= user.Hierarchy)
            return true;
        return false;
    }
    public static bool HasHigherHierarchy(this IGuildUser target, IGuildUser user)
    {
        if (target.Guild.OwnerId == target.Id
            || target.GuildPermissions.Administrator
            || target.Hierarchy >= user.Hierarchy)
            return true;
        return false;
    }
    public static bool CanAccess(this IGuildChannel channel, IGuildUser user)
    {
        var channels = (channel.Guild as SocketGuild).Channels.Where(x => x.Users.Contains(user));
        
        if (channels.Any(x => x.Id == channel.Id))
            return true;
        return false;
    }
}