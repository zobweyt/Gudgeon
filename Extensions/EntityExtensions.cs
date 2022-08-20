using Discord;

namespace Gudgeon;

internal static class EntityExtensions
{
    public static bool HasHigherHierarchy(this IRole role, bool respond = false, IDiscordInteraction? interaction = null)
    {
        bool condition = role.IsManaged 
            || role.Position >= role.Guild.GetCurrentUserAsync().Result.Hierarchy;
        return CheckHierarchy(condition, respond, interaction);
    }
    public static bool HasHigherHierarchy(this IGuildUser user, bool respond = false, IDiscordInteraction? interaction = null)
    {
        bool condition = user.Guild.OwnerId == user.Id 
            || user.GuildPermissions.Administrator 
            || user.Hierarchy >= user.Guild.GetCurrentUserAsync().Result.Hierarchy;
        return CheckHierarchy(condition, respond, interaction);
    }

    private static bool CheckHierarchy(bool condition, bool respond, IDiscordInteraction? interaction)
    {
        if (!condition)
            return false;

        if (respond)
            HierarchyErrorResponse(interaction);
        return true;
    }

    private static void HierarchyErrorResponse(IDiscordInteraction? interaction)
    {
        if (interaction == null)
            return;

        _ = interaction.ModifyWithStyleAsync(new ErrorStyle(), $"Target's hierarchy is higher that bot's.");
        _ = interaction.DelayedDeleteResponseAsync(TimeSpan.FromSeconds(10));
    }
}