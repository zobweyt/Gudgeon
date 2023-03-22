using Gudgeon.Data.Models;

namespace Gudgeon.Modules.Moderation;

[Group("role", "Role management")]
[RequireUserPermission(GuildPermission.ManageRoles)]
[RequireBotPermission(GuildPermission.ManageRoles)]
public class RolesModule : GudgeonModuleBase
{
    public RolesModule(InteractiveService interactiveService, GudgeonDbContext dbContext) 
        : base(interactiveService, dbContext)
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
        public RolesBulkModule(InteractiveService interactiveService, GudgeonDbContext dbContext)
            : base(interactiveService, dbContext)
        {
        }

        [SlashCommand("add", "Add a role to multiple guild members.")]
        public async Task<RuntimeResult> RoleBulkAddAsync(
            [Summary(description: "The role to add")][DoHierarchyCheck] IRole role,
            [Summary(description: "The type of users to add the role to")] GuildUsersType target)
            => await RoleBulkModifyAsync(false, role, target);

        [SlashCommand("remove", "Remove a role to multiple guild members.")]
        public async Task<RuntimeResult> RoleBulkRemoveAsync(
            [Summary(description: "The role to remove")][DoHierarchyCheck] IRole role,
            [Summary(description: "The type of users to remove the role from")] GuildUsersType target)
            => await RoleBulkModifyAsync(true, role, target);

        private async Task<RuntimeResult> RoleBulkModifyAsync(bool remove, IRole role, GuildUsersType target)
        {
            var members = Context.Guild.Users.Where(member => member.Roles.Contains(role) == remove && member.IsBot == (target == GuildUsersType.Bots));

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

    [RateLimit(seconds: 8)]
    [RequireUserPermission(GuildPermission.Administrator)]
    [Group("auto", "Automatic role assignment")]
    public class AutoroleModule : GudgeonModuleBase
    {
        public AutoroleModule(InteractiveService interactiveService, GudgeonDbContext dbContext)
            : base(interactiveService, dbContext)
        {
        }

        [SlashCommand("set", "Automatically add a role to new members with a specific target.")]
        public async Task<RuntimeResult> AutoroleAddAsync(
            [Summary(description: "The role to be added to new users on this server")][DoHierarchyCheck] IRole role,
            [Summary(description: "The type of users for which to add automatic role assignment")] GuildUsersType target)
        {
            if (_dbContext.Autoroles.Any(x => x.GuildId == Context.Guild.Id && x.UsersType == target))
                return GudgeonResult.FromError($"Automatic role assignment for new {target.ToString().ToLower()} is already set.");

            _dbContext.Add(new Autorole { GuildId = Context.Guild.Id, RoleId = role.Id, UsersType = target });
            await _dbContext.SaveChangesAsync();

            return GudgeonResult.FromSuccess($"Set automatic assignment of {role.Mention} for new {target.ToString().ToLower()}.");
        }

        [SlashCommand("remove", "Remove an automatic role assignment for a specific target of members.")]
        public async Task<RuntimeResult> AutoroleRemoveAsync([Summary(description: "The type of users for which to remove automatic role assignment")] GuildUsersType target)
        {
            Autorole? autorole = _dbContext.Autoroles.FirstOrDefault(x => x.GuildId == Context.Guild.Id && x.UsersType == target);

            if (autorole == null)
                return GudgeonResult.FromError($"No automatic role assignment for {target.ToString().ToLower()} is configured.");

            _dbContext.Remove(autorole);
            await _dbContext.SaveChangesAsync();

            return GudgeonResult.FromSuccess($"Removed automatic role assignment for new {target.ToString().ToLower()}.");
        }

        [SlashCommand("config", "Show all the configured automatic assignment roles.")]
        public async Task<RuntimeResult> ShowAsync()
        {
            IQueryable<Autorole> autoroles = _dbContext.Autoroles.Where(x => x.GuildId == Context.Guild.Id);

            if (!autoroles.Any())
                return GudgeonResult.FromError("No automatic assignment roles is configured in this guild.");

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Automatic assignment roles")
                .WithDescription("The roles to be added to new members with a specific target.")
                .WithFooter("You can always remove any them.")
                .WithColor(Colors.Primary);

            foreach (Autorole autorole in autoroles)
            {
                IRole role = Context.Guild.GetRole(autorole.RoleId);
                embedBuilder.AddField(autorole.UsersType.ToString(), role.Mention, true);
            }

            await RespondAsync(embed: embedBuilder.Build());
            return GudgeonResult.FromSuccess();
        }
    }
}