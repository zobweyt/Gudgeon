using Discord;

namespace Gudgeon;

internal static class InteractionExtensions
{
    public static async Task RespondWithStyleAsync(this IDiscordInteraction? interaction, EmbedStyle style, string message, bool autoDelete = false)
    {
        await interaction.RespondAsync(embed: style.BuildStyledEmbedMessage(message));

        if (autoDelete)
            await DelayedDeleteResponseAsync(interaction);
    }
    public static async Task ModifyWithStyleAsync(this IDiscordInteraction? interaction, EmbedStyle style, string message, bool autoDelete = false)
    {
        var response = await interaction.GetOriginalResponseAsync();
        if (response == null)
            return;

        void AddProperties(MessageProperties x)
        {                
            x.Embed = style.BuildStyledEmbedMessage(message);
            x.Components = new ComponentBuilder().Build();
        }

        if (interaction is IComponentInteraction component)
            await component.UpdateAsync(AddProperties);
        else
            await interaction.ModifyOriginalResponseAsync(AddProperties);

        if (autoDelete)
            await DelayedDeleteResponseAsync(interaction);
    }
    public static async Task DelayedDeleteResponseAsync(this IDiscordInteraction interaction)
    {
        await Task.Delay(TimeSpan.FromSeconds(8));
        var response = await interaction.GetOriginalResponseAsync();

        if (response != null)
            await response.DeleteAsync();
    }
}