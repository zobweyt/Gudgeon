using Fergun.Interactive.Pagination;

namespace Gudgeon.Pagination;

public class GudgeonStaticPaginator : BaseStaticPaginator
{
    public GudgeonStaticPaginator(GudgeonStaticPaginatorBuilder builder) 
        : base(builder)
    {
    }

    public override ComponentBuilder GetOrAddComponents(bool disableAll, ComponentBuilder? builder = null)
        => this.CustomizeComponents(disableAll);
}