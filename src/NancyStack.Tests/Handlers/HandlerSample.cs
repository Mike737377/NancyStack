using NancyStack.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Tests.Handlers
{
    public interface IBaseSampleHandler<TMessage, TReply> : IHandler<SampleHandlerContext, TMessage, TReply> { }
    public class SampleHandlerContext { }

    public class SampleHandlerInput { }
    public class SampleHandlerReply { }

    public class SampleHandler : IBaseSampleHandler<SampleHandlerInput, SampleHandlerReply>
    {
        public SampleHandlerReply Execute(SampleHandlerContext context, SampleHandlerInput message)
        {
            return new SampleHandlerReply();
        }
    }
}
