using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Handlers
{
    public interface IHandler<TMessage>
    {
        void Execute(IHandlerContext context, TMessage message);
    }

    public interface IHandler<TMessage, TReply>
    {
        TReply Execute(IHandlerContext context, TMessage message);
    }
}
