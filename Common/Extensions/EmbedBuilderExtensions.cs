using Discord;

namespace Gudgeon;

internal static class EmbedBuilderExtensions
{
    public static EmbedBuilder WithStyle(this EmbedBuilder builder, EmbedStyle style, string? name = null)
    {
        style.Apply(name);
        
        builder.WithAuthor(author =>
        {
            author
            .WithName(style.Name)
            .WithIconUrl(style.IconUrl);
        })
        .WithColor(style.Color);

        return builder;
    }
}