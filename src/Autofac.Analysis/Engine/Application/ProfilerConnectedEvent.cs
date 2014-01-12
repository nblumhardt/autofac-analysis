namespace Autofac.Analysis.Engine.Application
{
    public class ProfilerConnectedEvent
    {
        readonly string _name;
        readonly int _id;

        public ProfilerConnectedEvent(string name, int id)
        {
            _name = name;
            _id = id;
        }

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
