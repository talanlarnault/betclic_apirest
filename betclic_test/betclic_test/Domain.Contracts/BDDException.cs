using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace betclic_test.Domain.Contracts
{
    public class BDDException : Exception
    {
        public BDDException()
        {
        }

        public BDDException(string message) : base(message)
        {
        }

        public BDDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BDDException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
