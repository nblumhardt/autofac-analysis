namespace Autofac.Analysis.Engine.Application
{
    public class ItemCreatedEvent<TItem>
    {
        readonly TItem _item;

        public ItemCreatedEvent(TItem item)
        {
            _item = item;
        }

        public TItem Item
        {
            get { return _item; }
        }
    }
}
