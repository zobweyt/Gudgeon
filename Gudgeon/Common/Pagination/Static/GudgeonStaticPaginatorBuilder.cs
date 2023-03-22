using Fergun.Interactive.Pagination;

namespace Gudgeon.Pagination;

public class GudgeonStaticPaginatorBuilder : BaseStaticPaginatorBuilder<GudgeonStaticPaginator, GudgeonStaticPaginatorBuilder>
{
    public GudgeonStaticPaginatorBuilder() => this.Customize();

    public override GudgeonStaticPaginator Build() => new(this);
}