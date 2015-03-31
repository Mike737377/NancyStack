#Nancy Stack#

##Getting started##

- Create the handler context and layout

----

    public class HandlerContext
    {
		//add anything in here that you need passed around
    }

	public interface IHandler<TMessage, TReply> 
        : NancyStack.Handlers.IHandler<HandlerContext, TMessage, TReply>
    {
    }

----

- Initialise your wiring in the bootstrapper

----

    public class Bootstrapper : DefaultNancyBootstrapper
    {
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

----

- Create the handler

----

    public class IndexHandler : IHandler<IndexQueryModel, IndexViewModel>
    {
        public IndexViewModel Execute(HandlerContext context, IndexQueryModel message)
        {
            return new IndexViewModel { CurrentTime = DateTime.Now };
        }
    }

    public class IndexQueryModel { }

    public class IndexViewModel
    {
        public DateTime CurrentTime { get; set; }
    }

----

- When creating a module, inherit from NancyStackModule and wire in the constructor using Routes

----

    public class IndexModule : NancyStackModule
    {
        public IndexModule()
        {
            GetRoute<IndexQueryModel>("/")
                .Returning<IndexViewModel>()
                .OnSuccess(x => View["Index", x]);
		}
	}
 
----
