using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class KitchenUserNotFoundException : Exception
    {
        public KitchenUserNotFoundException() : base("No user found in this kitchen.")
        {
        }

        public KitchenUserNotFoundException(string message) : base(message)
        {
        }

        public KitchenUserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KitchenUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}