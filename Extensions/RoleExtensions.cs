using Discord.WebSocket;

namespace Gudgeon;

internal static class RoleExtensions
{
    public static bool IsHierarchyHigher(this SocketRole role)
    {
        if (role.IsManaged || role.IsEveryone || role.Position >= role.Guild.CurrentUser.Hierarchy)
            return true;
        return false;
    }
    public static bool IsHierarchyHigher(this SocketGuildUser user)
    {
        if (user.Guild.OwnerId == user.Id || user.GuildPermissions.Administrator || user.Hierarchy >= user.Guild.CurrentUser.Hierarchy)
            return true;
        return false;
    }
}