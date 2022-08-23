using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules.Roles;

[RequireUserPermission(GuildPermission.Administrator)]
[RequireBotPermission(GuildPermission.ManageRoles)]
public partial class RolesModule : GudgeonModuleBase
{
    public static int RoleApplyingTaskDelay { get { return 2000; } }
    public static int RoleApplyingDelay { get { return 500; } }

    [SlashCommand("add-multiple", "Add multiple roles to guild members", runMode: RunMode.Async)]
    public async Task<RuntimeResult> RoleMultipleAddAsync(
        [Summary("role", "The role to add")] IRole role,
        [Summary("query", "Whom to add the role")] RolesSelectQuery query,
        [Summary("ignore", "The role to be ignored")] IRole? ignore = null)
        => await RoleMultipleAsync(role, query, false, ignore);

    [SlashCommand("remove-multiple", "Remove multiple roles from guild members", runMode: RunMode.Async)]
    public async Task<RuntimeResult> RoleMultipleRemoveAsync(
        [Summary("role", "The role to remove")] IRole role,
        [Summary("query", "Whom to remove the role")] RolesSelectQuery query,
        [Summary("ignore", "The role to be ignored")] IRole? ignore = null)
        => await RoleMultipleAsync(role, query, true, ignore);

    public async Task<RuntimeResult> RoleMultipleAsync(IRole role, RolesSelectQuery query, bool remove = false, IRole? ignore = null)
    {
        await Context.Interaction.DeferAsync();

        if (role.HasHigherHierarchy(Context.Guild.CurrentUser)
            || role.HasHigherHierarchy(Context.User as IGuildUser))
            return GudgeonResult.FromError($"Unable to apply {role.Mention} due to its hierarchy.", TimeSpan.FromSeconds(10));

        var users = SelectUsers(role, query, remove, ignore);
        if (users.Count == 0)
            return GudgeonResult.FromError("No users found with these parameters.", TimeSpan.FromSeconds(12));

        int appliedRoles = await ApplyRolesAsync(users, role, remove);
        return GudgeonResult.FromSuccess($"Applied {appliedRoles} roles.", TimeSpan.FromSeconds(8));
    }

    private async Task<int> ApplyRolesAsync(List<SocketGuildUser> users, IRole role, bool remove)
    {
        DateTimeOffset endTime = DateTimeOffset.Now.AddMilliseconds(users.Count * (RoleApplyingDelay + RoleApplyingTaskDelay + Context.Client.Latency));
        await ModifyWithStyleAsync(new InfoStyle(), $"Changing roles for {users.Count} members will end at <t:{endTime.ToUnixTimeSeconds()}:T>");

        int appliedRoles = 0;
        foreach (var user in users)
        {
            if (user == null)
                continue;
            await ApplyRoleAsync(user, role, remove);
            appliedRoles++;
        }
        return appliedRoles;
    }
    private async Task ApplyRoleAsync(SocketGuildUser? user, IRole role, bool remove)
    {
        if (remove)
            await user.RemoveRoleAsync(role);
        else
            await user.AddRoleAsync(role);

        await Task.Delay(RoleApplyingTaskDelay + Context.Client.Latency);
    }

    private List<SocketGuildUser>? SelectUsers(IRole role, RolesSelectQuery query, bool remove, IRole? ignore)
    {
        var users = Context.Guild.Users.Where(x => 
        x.Roles.Contains(role) == remove 
        && !x.Roles.Contains(ignore));

        if (query.Equals(RolesSelectQuery.Admins))
            users = users.Where(x => x.GuildPermissions.Administrator == true);
        if (query.Equals(RolesSelectQuery.Bots))
            users = users.Where(x => x.IsBot == true);
        if (query.Equals(RolesSelectQuery.Humans))
            users = users.Where(x => x.IsBot == false);

        return users.ToList();
    }
}