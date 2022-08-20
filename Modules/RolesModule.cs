using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Modules;

[Group("role", "Role commands")]
[RequireBotPermission(GuildPermission.ManageRoles)]
[RequireUserPermission(GuildPermission.Administrator)]
public class RolesModule : GudgeonModuleBase
{
    public static int RoleApplyingTaskDelay { get { return 2000; } }
    public static int RoleApplyingDelay { get { return 500; } }

    [SlashCommand("add-everyone", "Adds the selected role to everyone")]
    public async Task<RuntimeResult> RoleAddEveryoneAsync(
        [Summary("role", "The role to add")] IRole role)
        => await RoleEveryoneAsync(role, false);

    [SlashCommand("remove-everyone", "Removes the selected role from everyone")]
    public async Task<RuntimeResult> RoleRemoveEveryoneAsync(
        [Summary("role", "The role to remove")] IRole role)
        => await RoleEveryoneAsync(role, true);

    [SlashCommand("add-bots", "Adds the selected role to every bot")]
    public async Task<RuntimeResult> RoleAddBotsAsync(
        [Summary("role", "The role to add")] IRole role)
        => await RoleEveryoneAsync(role, false, true);

    [SlashCommand("remove-bots", "Adds the selected role to every bot")]
    public async Task<RuntimeResult> RoleRemoveBotsAsync(
        [Summary("role", "The role to remove")] IRole role)
        => await RoleEveryoneAsync(role, true, true);

    public async Task<RuntimeResult> RoleEveryoneAsync(IRole role, bool remove, bool bots = false)
    {
        if (role.HasHigherHierarchy(true, Context.Interaction))
            return GudgeonResult.FromSuccess();

        List<SocketGuildUser>? users = Context.Guild.Users.Where(x => x.Roles.Contains(role) == remove).ToList();
        users = bots ? users.FindAll(x => x.IsBot) : users;

        await ApplyRolesAsync(users, role, remove);
        return GudgeonResult.FromSuccess($"Applied **{users.Count}** roles.", TimeSpan.FromSeconds(8));
    }

    private async Task ApplyRolesAsync(List<SocketGuildUser> users, IRole role, bool remove)
    {
        if (users.Count == 0)
            return;

        DateTimeOffset time = DateTimeOffset.Now.AddMilliseconds(users.Count * (RoleApplyingDelay + RoleApplyingTaskDelay + Context.Client.Latency));
        await ModifyWithStyleAsync(new ProcessingStyle(), $"Changing roles for **{users.Count}** members. Operation will end at <t:{time.ToUnixTimeSeconds()}:T> {Emojis.Animated.Loading}");

        foreach (var user in users)
            await ApplyRoleAsync(user, role, remove);
    }
    private async Task ApplyRoleAsync(SocketGuildUser? user, IRole role, bool remove)
    {
        if (user == null)
            return;

        if (remove)
            await user.RemoveRoleAsync(role);
        else
            await user.AddRoleAsync(role);

        await Task.Delay(RoleApplyingTaskDelay + Context.Client.Latency);
    }
}