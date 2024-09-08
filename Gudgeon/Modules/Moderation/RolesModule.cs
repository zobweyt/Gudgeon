using Discord.WebSocket;
using Gudgeon.Data.Models;

namespace Gudgeon.Modules.Moderation;

[Group("role", "Role management")]
[RequireUserPermission(GuildPermission.ManageRoles)]
[RequireBotPermission(GuildPermission.ManageRoles)]
public class RolesModule : GudgeonModuleBase
{
    public RolesModule(InteractiveService interactiveService) 
        : base(interactiveService)
    {
    }

    [SlashCommand("add", "Add a role to user.")]
    public async Task<RuntimeResult> RoleAddAsync(
        [Summary(description: "The user to give role")] IGuildUser user,
        [Summary(description: "The role to give")][DoHierarchyCheck] IRole role)
    {
        if (user.RoleIds.Any(x => x == role.Id))
            return GudgeonResult.FromError($"Cannot add {role.Mention} because {user.Mention} already have it.");

        await user.AddRoleAsync(role);
        return GudgeonResult.FromSuccess($"Added {role.Mention} to {user.Mention}.");
    }

    [SlashCommand("remove", "Remove a role from user.")]
    public async Task<RuntimeResult> RoleRemoveAsync(
        [Summary(description: "The user to remove role")] IGuildUser user,
        [Summary(description: "The role to remove")][DoHierarchyCheck] IRole role)
    {
        if (!user.RoleIds.Any(x => x == role.Id))
            return GudgeonResult.FromError($"Cannot remove {role.Mention} because {user.Mention} doesn't have it.");

        await user.RemoveRoleAsync(role);
        return GudgeonResult.FromSuccess($"Removed {role.Mention} from {user.Mention}.");
    }

    [Group("bulk", "Bulk role actions")]
    [RateLimit(seconds: 12)]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class RolesBulkModule : GudgeonModuleBase
    {
        public RolesBulkModule(InteractiveService interactiveService)
            : base(interactiveService)
        {
        }

        [SlashCommand("add", "Add a role to multiple guild members.")]
        public async Task<RuntimeResult> RoleBulkAddAsync(
            [Summary(description: "The role to add")][DoHierarchyCheck] IRole role,
            [Summary(description: "The type of users to add the role to")] GuildUsersType type)
            => await BulkModifyUsersAsync(m => m.AddRoleAsync(role), m => !m.Roles.Contains(role), type);

        [SlashCommand("remove", "Remove a role to multiple guild members.")]
        public async Task<RuntimeResult> RoleBulkRemoveAsync(
            [Summary(description: "The role to remove")][DoHierarchyCheck] IRole role,
            [Summary(description: "The type of users to remove the role from")] GuildUsersType type)
            => await BulkModifyUsersAsync(m => m.RemoveRoleAsync(role), m => m.Roles.Contains(role), type);

        private async Task<RuntimeResult> BulkModifyUsersAsync(Func<SocketGuildUser, Task> func,
            Func<SocketGuildUser, bool> predicate, GuildUsersType type)
        {
            var members = Context.Guild.Users.Where(member => predicate(member) && member.IsBot == (type == GuildUsersType.Bots));

            if (!members.Any())
                return GudgeonResult.FromError("No users found with these parameters.");

            await DeferAsync();

            foreach (var member in members)
                await func(member);

            return GudgeonResult.FromSuccess("Roles have been changed.");
        }
    }
}