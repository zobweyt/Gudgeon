namespace Gudgeon;

internal class ProcessingStyle : EmbedStyle
{
    public override void Apply(string? name = null)
    {
        Name = name ?? "Processing";
        IconUrl = Icons.Processing;
        Color = Colors.Processing;
    }
}