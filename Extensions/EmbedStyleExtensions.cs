using Discord;

namespace Gudgeon;

internal static class EmbedStyleExtensions
{
    public static Embed BuildStyledEmbedMessage(this EmbedStyle style, string message)
    {
        return new EmbedBuilder()
            .WithStyle(style)
            .WithDescription(message)
            .Build();
    }
}