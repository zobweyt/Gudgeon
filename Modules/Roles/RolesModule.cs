using Discord;
using Discord.Interactions;

namespace Gudgeon.Modules.Roles;

[RequireUserPermission(GuildPermission.Administrator)]
[RequireBotPermission(GuildPermission.ManageRoles)]
[Group("role", "Role commands")]
public class RolesModule : GudgeonModuleBase
{
    [SlashCommand("add", "Add a role to user")]
    public async Task<RuntimeResult> RoleAddAsync(
        [Summary("user", "The user to give role")] IGuildUser user,
        [Summary("role", "The role to give")][DoHierarchyCheck] IRole role)
    {
        if (user.RoleIds.Any(x => x == role.Id))
            return CommandResult.FromError($"Cannot add {role.Mention} because {user.Mention} already has it.");

        await user.AddRoleAsync(role);
        return CommandResult.FromSuccess($"Added {role.Mention} to {user.Mention}.", TimeSpan.FromSeconds(8));
    }

    [SlashCommand("remove", "Remove a role from user")]
    public async Task<RuntimeResult> RoleRemoveAsync(
        [Summary("user", "The user to remove role")] IGuildUser user,
        [Summary("role", "The role to remove")][DoHierarchyCheck] IRole role)
    {
        if (!user.RoleIds.Any(x => x == role.Id))
            return CommandResult.FromError($"Cannot remove {role.Mention} because {user.Mention} doesn't have it.", TimeSpan.FromSeconds(8));

        await user.RemoveRoleAsync(role);
        return CommandResult.FromSuccess($"Removed {role.Mention} from {user.Mention}.", TimeSpan.FromSeconds(8));
    }

    [DoMassUseCheck]
    [SlashCommand("multiple", "Multiple roles to guild members", runMode: RunMode.Async)]
    public async Task<RuntimeResult> RoleMultipleAsync(
            [Summary("action", "The action to apply")] RoleAction action,
            [Summary("role", "The role to remove")][DoHierarchyCheck] IRole role,
            [Summary("type", "The type of members")] GuildMembersType membersType)
    {
        var members = Context.Guild.Users.Where(
            x =>
            x.IsBot == (membersType == GuildMembersType.Bots) &&
            x.Roles.Contains(role) == (action == RoleAction.Remove))
            .ToList();

        if (members.Count == 0)
            return CommandResult.FromError("No users found with these parameters.", TimeSpan.FromSeconds(8));

        DateTimeOffset endTime = DateTimeOffset.UtcNow.AddMilliseconds(members.Count * (100 + Context.Client.Latency));
        await RespondWithStyleAsync(new InfoStyle(), $"Changing roles for {members.Count} members will end <t:{endTime.ToUnixTimeSeconds()}:R>");

        foreach (var member in members)
        {
            // should u delete this line ?
            if (member == null) continue;

            // what if the user leave the server ?
            try
            {
                await ApplyRoleAsync(member, role);
            }
            catch
            {

            }
            // wait 100ms
            // RateLimit ?? what  if we use this command in a huge server.
            await Task.Delay(100);
        }

        return CommandResult.FromSuccess($"Processed {members.Count} members.", TimeSpan.FromSeconds(12));
    }

    private async Task ApplyRoleAsync(IGuildUser? member, IRole role)
    {
        if (member.RoleIds.Any(x => x == role.Id))
            await member.RemoveRoleAsync(role);
        else
            await member.AddRoleAsync(role);
    }
}