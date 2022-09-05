using Discord;

namespace Gudgeon.Common.Styles;

public abstract class EmbedStyle
{
    public string? Name { get; protected set; }
    public string? IconUrl { get; protected set; }
    public Color Color { get; protected set; }

    public abstract void Apply(string? name = null);
}