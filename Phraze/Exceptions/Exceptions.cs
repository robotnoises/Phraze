using System;
using System.Runtime.Serialization;
using System.Text;

namespace Phraze.Exceptions
{
    public class EmptyPhrazeException : Exception, ISerializable
    {
        public EmptyPhrazeException() :base()
        {
            // Todo...
        }

        public EmptyPhrazeException(string message) : base(message)
        { 
            // Todo...
        }

        protected EmptyPhrazeException(SerializationInfo info, StreamingContext context)
        { 
            // Todo...
        }
    }
}
