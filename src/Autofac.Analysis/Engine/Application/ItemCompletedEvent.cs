namespace Autofac.Analysis.Engine.Application
{
    public class ItemCompletedEvent<TItem>
    {
        public ItemCompletedEvent(TItem item)
        {
            Item = item;
        }

        public TItem Item { get; }
    }
}
