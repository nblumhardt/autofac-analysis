namespace Autofac.Analysis.Engine.Application
{
    public class ItemCompletedEvent<TItem>
    {
        readonly TItem _item;

        public ItemCompletedEvent(TItem item)
        {
            _item = item;
        }

        public TItem Item
        {
            get { return _item; }
        }
    }
}
