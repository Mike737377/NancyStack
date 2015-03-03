using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NancyStack.Sample.Handlers
{
    public interface IHandler<TMessage, TReply> 
        : NancyStack.Handlers.IHandler<HandlerContext, TMessage, TReply>
    {
    }
}