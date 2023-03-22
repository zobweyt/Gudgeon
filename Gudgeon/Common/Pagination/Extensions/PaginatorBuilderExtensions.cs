using Fergun.Interactive.Pagination;

namespace Gudgeon.Pagination;

internal static class PaginatorBuilderExtensions
{
    public static TBuilder Customize<TPaginator, TBuilder>(this PaginatorBuilder<TPaginator, TBuilder> builder)
        where TPaginator : Paginator
        where TBuilder : PaginatorBuilder<TPaginator, TBuilder>
    {
        builder.WithActionOnCancellation(ActionOnStop.DeleteMessage);
        builder.WithActionOnTimeout(ActionOnStop.DisableInput);

        builder.Options.Clear();

        builder.AddOption(Emote.Parse(Emojis.Backward), PaginatorAction.Backward);
        builder.AddOption(Emote.Parse(Emojis.Exit), PaginatorAction.Exit);
        builder.AddOption(Emote.Parse(Emojis.Forward), PaginatorAction.Forward);

        builder.WithFooter(PaginatorFooter.None);

        return (TBuilder)builder;
    }
}