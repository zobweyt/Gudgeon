using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules;

[Group("role", "Role commands")]
[RequireBotPermission(GuildPermission.ManageRoles)]
[RequireUserPermission(GuildPermission.Administrator)]
public class RolesModule : GudgeonModuleBase
{
    [SlashCommand("add-everyone", "Adds the selected role to everyone")]
    public async Task<RuntimeResult> RoleAddEveryoneAsync(
        [Summary("role", "The role to add")] SocketRole role)
        => await RoleEveryoneAsync(role, false);

    [SlashCommand("remove-everyone", "Removes the selected role from everyone")]
    public async Task<RuntimeResult> RoleRemoveEveryoneAsync(
        [Summary("role", "The role to remove")] SocketRole role)
        => await RoleEveryoneAsync(role, true);

    public async Task<RuntimeResult> RoleEveryoneAsync(SocketRole role, bool remove)
    {
        if (role.IsHierarchyHigher())
            return GudgeonResult.FromError($"{role.Mention} hierarchy is higher than bot's.");
        
        var users = Context.Guild.Users.Where(x => x.IsBot == false && x.GuildPermissions.Administrator == false && x.Roles.Contains(role) == remove).ToList();
        await ApplyRolesAsync(users, role, remove);

        return GudgeonResult.FromSuccess($"Applied **{users.Count}** roles.", TimeSpan.FromSeconds(8));
    }

    private async Task ApplyRolesAsync(List<SocketGuildUser> users, SocketRole role, bool remove)
    {
        await ModifyWithStyleAsync(new ProcessingStyle(), $"Changing roles for **{users.Count}** members {Emojis.Animated.Loading}");
        
        foreach (var user in users)
            await ApplyRoleAsync(user, role, remove);
    }
    private async Task ApplyRoleAsync(SocketGuildUser? user, SocketRole role, bool remove)
    {
        if (user == null)
            return;

        if (remove)
            await user.RemoveRoleAsync(role);
        else
            await user.AddRoleAsync(role);
    }
}