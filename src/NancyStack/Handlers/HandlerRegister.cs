using NancyStack.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NancyStack.Handlers
{
    public interface IHandlerRegistry
    {
        TReply Execute<TMessage, TReply>(TMessage message);
    }

    public class HandlerRegister : IHandlerRegistration, IHandlerRegistry
    {
        private static readonly Type baseHandlerType = typeof(IHandler<,,>);
        private static readonly Dictionary<Type, HandlerRegistration> registeredHandlerTypes = new Dictionary<Type, HandlerRegistration>();
        private static readonly Func<Type, bool> isHandler = (i) => i.IsGenericType && i.GetGenericTypeDefinition() == baseHandlerType;
        private static Func<object> handlerContextFactory = () => { throw new NancyStackWiringMissingException("HandlerContextFactory"); };

        public IHandlerRegistration ScanAssembly(Assembly assembly)
        {
            var handlers = assembly.GetTypes().Where(x =>
                x.IsClass && !x.IsAbstract &&
                x.GetInterfaces().Any(isHandler)).ToArray();

            handlers.Each(x => RegisterHandler(x));

            return this;
        }

        public IHandlerRegistration ScanAssemblyContainingType(Type containingType)
        {
            return ScanAssembly(containingType.Assembly);
        }

        public IHandlerRegistration ScanAssemblyContainingType<T>()
        {
            return ScanAssembly(typeof(T).Assembly);
        }

        public IHandlerRegistration RegisterHandler<THandler>()
        {
            return RegisterHandler(typeof(THandler));
        }

        public IHandlerRegistration RegisterHandler(Type handlerType)
        {
            var handlerDeclarations = handlerType.GetInterfaces().Where(isHandler).ToList();

            handlerDeclarations.Each(x =>
            {
                var r = new HandlerRegistration(x, handlerType);
                registeredHandlerTypes[r.MessageType] = r;
            });

            return this;
        }

        public HandlerRegistration GetHandlerRegistration<TMessage>()
        {
            var messageType = typeof(TMessage);
            if (!registeredHandlerTypes.ContainsKey(messageType))
            {
                throw new HandlerNotFound(messageType);
            }

            return registeredHandlerTypes[messageType];
        }

        public TReply Execute<TMessage, TReply>(TMessage message)
        {
            var messageType = typeof(TMessage);

            if (!registeredHandlerTypes.ContainsKey(messageType))
            {
                throw new HandlerNotFound(messageType);
            }

            var context = CreateHandlerContext();
            return registeredHandlerTypes[messageType].Execute<TMessage, TReply>(context, message);
        }

        public void RegisterHandlerContextFactory(Func<object> factory)
        {
            handlerContextFactory = factory;
        }

        public object CreateHandlerContext()
        {
            return handlerContextFactory();
        }
    }
}