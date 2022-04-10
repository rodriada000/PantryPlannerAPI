using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class KitchenListNotFoundException : Exception
    {
        public KitchenListNotFoundException(long kitchenId) : base($"grocery list with ID {kitchenId} does not exist.")
        {           
        }

        public KitchenListNotFoundException(string message) : base(message)
        {
        }

        public KitchenListNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KitchenListNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}