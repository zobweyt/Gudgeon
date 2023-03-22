using Fergun.Interactive.Pagination;

namespace Gudgeon.Pagination;

public class GudgeonLazyPaginatorBuilder : BaseLazyPaginatorBuilder<GudgeonLazyPaginator, GudgeonLazyPaginatorBuilder>
{
    public GudgeonLazyPaginatorBuilder() => this.Customize();
    
    public override GudgeonLazyPaginator Build() => new(this);
}