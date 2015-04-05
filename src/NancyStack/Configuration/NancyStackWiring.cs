using Nancy.Bootstrapper;
using Nancy.Security;
using NancyStack.Handlers;
using NancyStack.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NancyStack.Configuration
{
    public interface IWire
    {
        IWire RegisterHandlers(Action<IHandlerRegistration> handlerRegistration);

        IWire EnableCsrf(IPipelines pipelines);
    }

    public interface IHandlerRegistration
    {
        IHandlerRegistration ScanAssembly(Assembly assembly);

        IHandlerRegistration ScanAssemblyContainingType(Type containingType);

        IHandlerRegistration ScanAssemblyContainingType<T>();

        IHandlerRegistration RegisterHandler<THandler>();

        IHandlerRegistration RegisterHandler(Type handlerType);

        void RegisterHandlerContextFactory(Func<object> factory);
    }

    public class HandlerRegistration
    {
        private static readonly Type baseHandlerType = typeof(IHandler<,,>);

        public HandlerRegistration(Type handlerInterface, Type handlerType)
        {
            var args = handlerInterface.GetGenericArguments();
            MessageType = args[1];
            HandlerType = handlerType;
            HandlerContextType = args[0];
            Executor = handlerInterface.GetMethod("Execute");
        }

        public Type MessageType { get; private set; }
        public Type HandlerType { get; private set; }
        public Type HandlerContextType { get; private set; }
        private MethodInfo Executor { get; set; }

        public TReply Execute<TMessage, TReply>(object context, TMessage message)
        {
            var handler = Activator.CreateInstance(HandlerType);
            var response = Executor.Invoke(handler, new object[] { context, message });
            return (TReply)response;
        }
    }

    public class HandlerNotFound : NancyStackException
    {
        public HandlerNotFound(Type messageType)
            : base(string.Format("Could not find hander for type '{0}'", messageType.FullName))
        { }
    }

    public class Wiring : IWire
    {
        private readonly IHandlerRegistration handlerRegister;

        public Wiring(IHandlerRegistration handlerRegister)
        {
            this.handlerRegister = handlerRegister;
        }

        public IWire RegisterHandlers(Action<IHandlerRegistration> handlerRegistration)
        {
            handlerRegistration(handlerRegister);
            return this;
        }

        public IWire EnableCsrf(IPipelines pipelines)
        {
            NancyStackWiring.ConfigurationOptions.CsrfEnabled = true;
            Csrf.Enable(pipelines);
            return this;
        }
    }

    public static class NancyStackWiring
    {
        static NancyStackWiring()
        {
            HandlerRegistry = new HandlerRegister();
            ConfigurationOptions = new ConfigurationOptions();
        }

        public static IHandlerRegistry HandlerRegistry { get; private set; }
        public static ConfigurationOptions ConfigurationOptions { get; private set; }

        public static void ConfigureNancyStack(this INancyBootstrapper bootstrapper, Action<IWire> config)
        {
            config(new Wiring(HandlerRegistry as IHandlerRegistration));
        }
    }

    public class ConfigurationOptions
    {
        public bool CsrfEnabled { get; set; }
    }

    public class NancyStackWiringMissingException : NancyStackException
    {
        public NancyStackWiringMissingException(string propertyName)
            : base(string.Format("Wiring missing for '{0}'", propertyName))
        { }
    }
}