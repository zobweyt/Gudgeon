using Discord.Rest;

namespace Gudgeon;

/// <summary>
/// Provides extension methods for <see cref="BaseDiscordClient"/>.
/// </summary>
internal static class BaseDiscordClientExtensions
{
    /// <summary>
    /// Gets the bot invitation link depending on the <paramref name="permissions"/>.
    /// </summary>
    /// <param name="client">The current client.</param>
    /// <param name="permissions">The calculated permissions.</param>
    public static string GetBotInviteUrl(this BaseDiscordClient client, ulong permissions = 1099981663238)
    {
        return $"https://discord.com/api/oauth2/authorize?client_id={client.CurrentUser.Id}&permissions={permissions}&scope=bot%20applications.commands";
    }
}