using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NancyStack
{
    [Serializable]
    public class NancyStackException : Exception
    {

        public NancyStackException()
            : base()
        { }

        public NancyStackException(string message)
            : base(message)
        { }

        public NancyStackException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public NancyStackException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
