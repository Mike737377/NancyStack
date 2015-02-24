using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Handlers
{
    public interface IInitHandlerContext
    {
        IHandlerContext Init(IApplicationBus bus);
    }

    public interface IHandlerContext : IInitHandlerContext
    {
        IDataContext Database { get; }
        IApplicationBus Bus { get; }
        IUserContext User { get; }
    }

    public interface IUserContext
    {
    }

    public interface IDataContext
    {
    }

    public class HandlerContext : IInitHandlerContext, IHandlerContext
    {
        public HandlerContext(IDataContext database, IUserContext userContext)
        {
            Database = database;
            User = userContext;
        }

        public IDataContext Database { get; private set; }
        public IUserContext User { get; private set; }
        public IApplicationBus Bus { get; private set; }

        public IHandlerContext Init(IApplicationBus bus)
        {
            Bus = bus;
            return this;
        }
    }
}
