using Discord;

namespace Gudgeon;

internal static class InteractionExtensions
{
    private delegate Task Respond(IDiscordInteraction interaction, Embed embed);
    private static async Task RespondAsync(Respond respond, IDiscordInteraction interaction, EmbedStyle style, string? message)
    {
        var embed = new EmbedBuilder()
            .WithStyle(style)
            .WithDescription(message ?? string.Empty)
            .Build();

        await respond(interaction, embed);
    }
    private static async Task SendStyleAsync(IDiscordInteraction interaction, Embed embed)
        => await interaction.RespondAsync(embed: embed);
    private static async Task ModifyStyleAsync(IDiscordInteraction interaction, Embed embed)
    {
        var response = await interaction.GetOriginalResponseAsync();
        if (response == null)
            return;

        void AddProperties(MessageProperties x)
        {
            x.Embed = embed;
            x.Components = new ComponentBuilder().Build();
        }

        if (interaction is IComponentInteraction component)
        {
            await component.UpdateAsync(AddProperties);
            return;
        }
        await interaction.ModifyOriginalResponseAsync(AddProperties);
    }

    public static async Task RespondWithStyleAsync(this IDiscordInteraction interaction, EmbedStyle style, string? message)
        => await RespondAsync(SendStyleAsync, interaction, style, message);
    public static async Task ModifyWithStyleAsync(this IDiscordInteraction interaction, EmbedStyle style, string? message)
        => await RespondAsync(ModifyStyleAsync, interaction, style, message);
    public static async Task DelayedDeleteResponseAsync(this IDiscordInteraction interaction, TimeSpan span)
    {
        await Task.Delay(span);
        var response = await interaction.GetOriginalResponseAsync();

        if (response != null)
            await response.DeleteAsync();
    }
}