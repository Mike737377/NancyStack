using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing
{
    public class HandlerMissingException : Exception
    {
        public HandlerMissingException(string route, Type inputModel, Type outputModel, Exception innerException)
            : base(string.Format("Route '{0}' is missing handle IHandler<{1},{2}>", route, inputModel.FullName, outputModel.FullName), innerException)
        {
        }
    }
}
