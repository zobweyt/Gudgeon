namespace Gudgeon.Common.Styles;

internal class ErrorStyle : EmbedStyle
{
    public override void Apply(string? name = null)
    {
        Name = name ?? "Error";
        IconUrl = Icons.Error;
        Color = Colors.Danger;
    }
}