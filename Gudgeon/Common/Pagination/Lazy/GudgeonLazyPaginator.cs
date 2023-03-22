using Fergun.Interactive.Pagination;

namespace Gudgeon.Pagination;

public class GudgeonLazyPaginator : BaseLazyPaginator
{
    public GudgeonLazyPaginator(GudgeonLazyPaginatorBuilder builder) 
        : base(builder)
    {
    }

    public override ComponentBuilder GetOrAddComponents(bool disableAll, ComponentBuilder? builder = null)
        => this.CustomizeComponents(disableAll);
}