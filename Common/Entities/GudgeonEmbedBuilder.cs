using Discord;

namespace Gudgeon;

internal class GudgeonEmbedBuilder : EmbedBuilder
{
	public GudgeonEmbedBuilder()
	{
		WithColor(Colors.Default);
	}
}