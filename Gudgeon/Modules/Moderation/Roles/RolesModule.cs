using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Gudgeon.Modules.Moderation.Roles;

[Group("role", "Role commands")]
[RequireUserPermission(GuildPermission.ManageRoles)]
[RequireBotPermission(GuildPermission.ManageRoles)]
public class RolesModule : GudgeonModuleBase
{
    public RolesModule(InteractiveService interactive)
        : base(interactive)
    {
    }

    [SlashCommand("add", "Add a role to user")]
    public async Task<RuntimeResult> RoleAddAsync(
        [Summary("user", "The user to give role")] IGuildUser user,
        [Summary("role", "The role to give")][DoHierarchyCheck] IRole role)
    {
        if (user.RoleIds.Any(x => x == role.Id))
            return GudgeonResult.FromError($"Cannot add {role.Mention} because {user.Mention} already have it.");

        await user.AddRoleAsync(role);
        return GudgeonResult.FromSuccess($"Added {role.Mention} to {user.Mention}.");
    }

    [SlashCommand("remove", "Remove a role from user")]
    public async Task<RuntimeResult> RoleRemoveAsync(
        [Summary("user", "The user to remove role")] IGuildUser user,
        [Summary("role", "The role to remove")][DoHierarchyCheck] IRole role)
    {
        if (!user.RoleIds.Any(x => x == role.Id))
            return GudgeonResult.FromError($"Cannot remove {role.Mention} because {user.Mention} doesn't have it.");

        await user.RemoveRoleAsync(role);
        return GudgeonResult.FromSuccess($"Removed {role.Mention} from {user.Mention}.");
    }

    [RateLimit(seconds: 12, requests: 1)]
    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("bulk", "Modify role of multiple guild members")]
    public async Task<RuntimeResult> RoleMultipleAsync(
        [Summary("action", "Desired action with role")] BulkAction action,
        [Summary("role", "The role to apply")][DoHierarchyCheck] IRole role,
        [Summary("members", "Guild members to modify")] MembersType target = MembersType.Everyone)
    {
        bool remove = action == BulkAction.Remove;
        var members = Context.Guild.Users.Where(member => member.Roles.Contains(role) == remove);

        if (target != MembersType.Everyone)
            members = members.Where(member => member.IsBot == (target == MembersType.Bots));        

        if (!members.Any())
            return GudgeonResult.FromError("No users found with these parameters.");
        
        await DeferAsync();

        foreach (var member in members)
        {
            if (remove)
                await member.RemoveRoleAsync(role);
            else
                await member.AddRoleAsync(role);
        }

        return GudgeonResult.FromSuccess("Roles have been changed.");
    }
}