﻿using Nancy.Bootstrapper;
using NancyStack.Handlers;
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

    public interface IHandlerRegister
    {
        TReply ExecuteHandlerFor<TMessage, TReply>(TMessage message);
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


    public class HandlerRegister : IHandlerRegistration, IHandlerRegister
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

        public TReply ExecuteHandlerFor<TMessage, TReply>(TMessage message)
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
    }

    public static class NancyStackWiring
    {

        static NancyStackWiring()
        {
            HandlerRegister = new HandlerRegister();
        }

        public static IHandlerRegister HandlerRegister { get; set; }

        public static void ConfigureNancyStack(this INancyBootstrapper bootstrapper, Action<IWire> config)
        {
            config(new Wiring(HandlerRegister as IHandlerRegistration));
        }
    }

    public class NancyStackWiringMissingException : NancyStackException
    {
        public NancyStackWiringMissingException(string propertyName)
            : base(string.Format("Wiring missing for '{0}'", propertyName))
        { }
    }
}