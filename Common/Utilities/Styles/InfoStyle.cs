namespace Gudgeon;

internal class InfoStyle : EmbedStyle
{
    public override void Apply(string? name = null)
    {
        Name = name ?? "Info";
        IconUrl = Icons.Info;
        Color = Colors.Info;
    }
}