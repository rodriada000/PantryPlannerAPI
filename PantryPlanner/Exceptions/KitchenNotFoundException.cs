using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class KitchenNotFoundException : Exception
    {
        public KitchenNotFoundException(long kitchenId) : base($"kitchen with ID {kitchenId} does not exist.")
        {           
        }

        public KitchenNotFoundException(string message) : base(message)
        {
        }

        public KitchenNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KitchenNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}