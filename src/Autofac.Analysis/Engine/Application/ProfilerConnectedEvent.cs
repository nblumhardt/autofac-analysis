namespace Autofac.Analysis.Engine.Application
{
    public class ProfilerConnectedEvent
    {
        public ProfilerConnectedEvent(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public int Id { get; }

        public string Name { get; }
    }
}
