using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class InviteNotFoundException : Exception
    {
        public InviteNotFoundException(long kitchenId) : base($"No invite found for kitchen ID {kitchenId}.")
        {
        }

        public InviteNotFoundException(string message) : base(message)
        {
        }

        public InviteNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InviteNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
