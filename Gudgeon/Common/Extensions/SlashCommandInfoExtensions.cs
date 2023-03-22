namespace Gudgeon;

/// <summary>
/// Provides extension methods for <see cref="SlashCommandInfo"/>.
/// </summary>
internal static class SlashCommandInfoExtensions
{
    /// <summary>
    /// Gets a mention of this command using the IDs of commands in the current context.
    /// </summary>
    /// <param name="command">The slash command to mention.</param>
    /// <param name="commands">A collection of slash commands in the current context.</param>
    /// <returns>Discord formated slash command mention.</returns>
    public static string GetMention(this SlashCommandInfo command, IReadOnlyCollection<IApplicationCommand> commands)
    {
        string name = command.Name;
        ModuleInfo module = command.Module;

        while (!commands.Any(x => x.Name == name))
        {
            name = module.SlashGroupName;
            module = module.Parent;
        }

        ulong id = commands.First(x => x.Name == name).Id;
        return $"</{command}:{id}>";
    }
}