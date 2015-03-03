using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Handlers
{
    public interface IHandler<THandlerContext, TMessage, TReply>
    {
        TReply Execute(THandlerContext context, TMessage message);
    }
}
