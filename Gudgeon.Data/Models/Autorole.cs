namespace Gudgeon.Data.Models;

/// <summary>
/// Responsible for automatically assigning roles to <see cref="UsersType"/> when joining to the <see cref="GuildId"/>.
/// </summary>
public class Autorole
{
    /// <summary>
    /// The ID of this entity in the database.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The Discord guild this autorole is set for.
    /// </summary>
    public ulong GuildId { get; set; }

    /// <summary>
    /// The Discord role ID to add to the <see cref="UsersType"/>.
    /// </summary>
    public ulong RoleId { get; set; }

    /// <summary>
    /// Determines if the autorole should only work for bots
    /// </summary>
    public GuildUsersType UsersType { get; set; }
}