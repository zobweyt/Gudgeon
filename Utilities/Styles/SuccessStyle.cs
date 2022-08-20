namespace Gudgeon;

internal class SuccessStyle : EmbedStyle
{
    public override void Apply(string? name = null)
    {
        Name = name ?? "Success";
        IconUrl = Icons.Success;
        Color = Colors.Success;
    }
}