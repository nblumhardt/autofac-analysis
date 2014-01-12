namespace Autofac.Analysis.Engine.Application
{
    public interface IActiveItemRepository<TItem>
        where TItem : IApplicationItem
    {
        bool TryGetItem(string id, out TItem item);
        void Add(TItem item);
        void RemoveCompleted(TItem item);
    }
}
