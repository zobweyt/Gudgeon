﻿using Discord;
using Discord.Interactions;
using Gudgeon.Common.Styles;

namespace Gudgeon.Modules.Roles;

[RequireUserPermission(GuildPermission.Administrator)]
[RequireBotPermission(GuildPermission.ManageRoles)]
[Group("role", "Role commands")]
public class RolesModule : GudgeonModuleBase
{
    [SlashCommand("add", "Add a role to user")]
    public async Task<RuntimeResult> RoleAddAsync(
        [Summary("user", "The user to give role")] IGuildUser user,
        [Summary("role", "The role to give")] [DoHierarchyCheck] IRole role)
    {
        if (user.RoleIds.Any(x => x == role.Id))
            return GudgeonResult.FromError($"Cannot add {role.Mention} because {user.Mention} already has it.");

        await user.AddRoleAsync(role);
        return GudgeonResult.FromSuccess($"Added {role.Mention} to {user.Mention}.");
    }

    [SlashCommand("remove", "Remove a role from user")]
    public async Task<RuntimeResult> RoleRemoveAsync(
        [Summary("user", "The user to remove role")] IGuildUser user,
        [Summary("role", "The role to remove")] [DoHierarchyCheck] IRole role)
    {
        if (!user.RoleIds.Any(x => x == role.Id))
            return GudgeonResult.FromError($"Cannot remove {role.Mention} because {user.Mention} doesn't have it.");
        
        await user.RemoveRoleAsync(role);
        return GudgeonResult.FromSuccess($"Removed {role.Mention} from {user.Mention}.");
    }

    [DoMassUseCheck]
    [SlashCommand("multiple", "Multiple roles to guild members", runMode: RunMode.Async)]
    public async Task<RuntimeResult> RoleMultipleAsync(
            [Summary("action", "The action to apply")] RoleAction action,
            [Summary("role", "The role to remove")] [DoHierarchyCheck] IRole role,
            [Summary("type", "The type of members")] GuildMembersType membersType)
    {
        var members = Context.Guild.Users.Where(x => x.Roles.Contains(role) == (action == RoleAction.Remove));
        if (membersType != GuildMembersType.Everyone)
            members = members.Where(x => x.IsBot == (membersType == GuildMembersType.Bots));

        int membersCount = members.Count();
        if (membersCount == 0)
            return GudgeonResult.FromError("No users found with these parameters.");

        DateTimeOffset endTime = DateTimeOffset.UtcNow.AddMilliseconds(membersCount * 1100 + Context.Client.Latency);
        var embed = new EmbedBuilder()
            .WithStyle(new InfoStyle())
            .WithDescription($"Changing roles for {membersCount} members will end around at <t:{endTime.ToUnixTimeSeconds()}:T>")
            .Build();

        await Context.Interaction.RespondAsync(embed: embed);

        foreach (var member in members)
        {
            if (member == null) continue;
            await ApplyRoleAsync(member, role, action == RoleAction.Remove);
        }

        return GudgeonResult.FromSuccess($"Roles for {membersCount} members have been changed.");
    }

    private async Task ApplyRoleAsync(IGuildUser? member, IRole role, bool remove)
    {
        bool hasRole = member.RoleIds.Any(x => x == role.Id);

        if (remove && hasRole)
        {
            await member.RemoveRoleAsync(role);
            return;
        }

        if (!hasRole)
            await member.AddRoleAsync(role);

        await Task.Delay(100);
    }
}