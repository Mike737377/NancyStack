using Nancy;
using NancyStack.Configuration;
using NancyStack.Sample.Handlers;

namespace NancyStack.Sample
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            this.ConfigureNancyStack(config =>
            {
                config.RegisterHandlers(r => r
                    .ScanAssemblyContainingType<Bootstrapper>()
                    .RegisterHandlerContextFactory(() => new HandlerContext()))
                    .EnableCsrf(pipelines);
            });

            base.ApplicationStartup(container, pipelines);
        }
    }
}