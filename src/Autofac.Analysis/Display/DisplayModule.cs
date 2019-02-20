using System;
using Serilog;

namespace Autofac.Analysis.Display
{
    class DisplayModule : Module
    {
        readonly ILogger _logger;

        public DisplayModule(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<EventWriter>().SingleInstance();
            builder.RegisterInstance(_logger.ForContext("AutofacAnalysisSessionId", Guid.NewGuid().ToString("n"))).ExternallyOwned();
        }
    }
}
