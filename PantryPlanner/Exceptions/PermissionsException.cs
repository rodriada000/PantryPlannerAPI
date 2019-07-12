using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class PermissionsException : Exception
    {
        public PermissionsException() : base("User does not have rights to kitchen.")
        {
        }

        public PermissionsException(string message) : base(message)
        {
        }

        public PermissionsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PermissionsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}