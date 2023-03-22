namespace Gudgeon.Styles;

/// <summary>
/// Provides extension methods for <see cref="EmbedBuilder"/>.
/// </summary>
internal static class EmbedBuilderExtensions
{
    /// <summary>
    /// Sets the <see cref="EmbedStyle"/> of the embed.
    /// </summary>
    /// <param name="builder">The current builder.</param>
    /// <param name="style">An <see cref="EmbedStyle"/> to set.</param>
    /// <param name="name">The <see cref="EmbedStyle.Name"/>, otherwise style name if null.</param>
    /// <returns>The current builder.</returns>
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