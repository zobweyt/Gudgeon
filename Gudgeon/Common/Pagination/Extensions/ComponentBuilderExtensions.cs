using Fergun.Interactive.Pagination;

namespace Gudgeon;

internal static class PaginatorExtensions
{
    public static ComponentBuilder CustomizeComponents(this Paginator paginator, bool disableAll)
    {
        var builder = new ComponentBuilder();
        builder.WithButton($"Page {paginator.CurrentPageIndex + 1} / {paginator.MaxPageIndex + 1}", "page_of", disabled: true);

        foreach (var emote in paginator.Emotes)
        {
            bool isDisabled = disableAll || emote.Value switch
            {
                PaginatorAction.Backward => paginator.CurrentPageIndex == 0,
                PaginatorAction.Forward => paginator.CurrentPageIndex == paginator.MaxPageIndex,
                _ => false
            };

            var button = new ButtonBuilder()
                .WithCustomId(emote.Key.ToString())
                .WithStyle(ButtonStyle.Secondary)
                .WithEmote(emote.Key)
                .WithDisabled(isDisabled);

            builder.WithButton(button);
        }

        return builder;
    }
}