namespace Gudgeon;

/// <summary>
/// Provides extension methods for <see cref="ModuleInfo"/>.
/// </summary>
internal static class ModuleInfoExtensions
{
    /// <summary>
    /// Gets a collection of slash commands in submodules of this module.
    /// </summary>
    /// <param name="module">The module to look in.</param>
    /// <returns>
    /// A read-only collection of slash commands found in submodules, including the module itself.
    /// </returns>
    public static IReadOnlyCollection<SlashCommandInfo> GetSlashCommandsInSubModules(this ModuleInfo module)
    {
        List<SlashCommandInfo> commands = new();

        foreach (SlashCommandInfo command in module.SlashCommands)
        {
            commands.Add(command);
        }
        foreach (ModuleInfo subModule in module.SubModules)
        {
            commands.AddRange(GetSlashCommandsInSubModules(subModule));
        }

        return commands.AsReadOnly();
    }
}