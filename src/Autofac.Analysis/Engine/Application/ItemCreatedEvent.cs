namespace Autofac.Analysis.Engine.Application
{
    public class ItemCreatedEvent<TItem>
    {
        public ItemCreatedEvent(TItem item)
        {
            Item = item;
        }

        public TItem Item { get; }
    }
}
