using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyStack.Routing
{
    public class RouteCouldNotBeFoundException : Exception
    {
        public RouteCouldNotBeFoundException(Type routeModel)
            : base(string.Format("Route could not be found for '{0}'", routeModel.FullName))
        { }
    }
}
