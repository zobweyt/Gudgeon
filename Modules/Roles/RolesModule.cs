using Discord;
using Discord.Interactions;

namespace Gudgeon.Modules.Roles;

[RequireUserPermission(GuildPermission.Administrator)]
[RequireBotPermission(GuildPermission.ManageRoles)]
[Group("role", "Role commands")]
public partial class RolesModule : GudgeonModuleBase
{
    [SlashCommand("add", "Add a role")]
    public async Task<RuntimeResult> RoleAddAsync(IGuildUser user, IRole role)
    {
        if (role.HasHigherHierarchy(Context.Guild.CurrentUser) 
            || role.HasHigherHierarchy(Context.User as IGuildUser))
            return GudgeonResult.FromError($"Unable to add {role.Mention} to {user.Mention} due to its hierarchy.", TimeSpan.FromSeconds(10));

        await user.AddRoleAsync(role);
        return GudgeonResult.FromSuccess($"Added {role.Mention} to {user.Mention}.", TimeSpan.FromSeconds(8));
    }

    [SlashCommand("remove", "Remove a role")]
    public async Task<RuntimeResult> RoleRemoveAsync(IGuildUser user, IRole role)
    {
        if (role.HasHigherHierarchy(Context.Guild.CurrentUser)
            || role.HasHigherHierarchy(Context.User as IGuildUser))
            return GudgeonResult.FromError($"Unable to remove {role.Mention} from {user.Mention} due to its hierarchy.", TimeSpan.FromSeconds(10));

        await user.RemoveRoleAsync(role);
        return GudgeonResult.FromSuccess($"Removed {role.Mention} from {user.Mention}.", TimeSpan.FromSeconds(8));
    }
}