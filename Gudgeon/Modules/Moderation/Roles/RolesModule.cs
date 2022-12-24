using Discord;
using Discord.Interactions;
using Fergun.Interactive;
using Gudgeon.Common.Styles;
using static Gudgeon.Modules.Moderation.Roles.MembersType;

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

    [RateLimit(seconds: 16, requests: 1)]
    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("multiple", "Multiple roles to guild members", runMode: RunMode.Async)]
    public async Task<RuntimeResult> RoleMultipleAsync(
            [Summary("action", "The action to apply roles")][Choice("Add", "add"), Choice("Remove", "remove")] string action,
            [Summary("role", "The role to remove")][DoHierarchyCheck] IRole role,
            [Summary("type", "The type of members")] MembersType membersType)
    {
        bool remove = action == "remove";
        var members = Context.Guild.Users.Where(member => member.Roles.Contains(role) == remove);
        members = membersType == Everyone ? members : members.Where(member => member.IsBot == (membersType == Bots));
        
        if (!members.Any())
            return GudgeonResult.FromError("No users found with these parameters.");

        DateTimeOffset endAt = DateTimeOffset.UtcNow.AddMilliseconds(members.Count() * 1000 + Context.Client.Latency);

        var embed = new EmbedBuilder()
            .WithStyle(new InfoStyle())
            .WithDescription($"Operation will end at <t:{endAt.ToUnixTimeSeconds()}:T>")
            .Build();

        await RespondAsync(embed: embed);

        foreach (var member in members)
        {
            if (remove)
            {
                await member.RemoveRoleAsync(role);
                continue;
            }
            await member.AddRoleAsync(role);
        }

        return GudgeonResult.FromSuccess("Roles have been changed.");
    }
}