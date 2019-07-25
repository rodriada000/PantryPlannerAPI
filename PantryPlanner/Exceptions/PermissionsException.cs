using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    /// <summary>
    /// Exception that can be thrown when a user does not have rights to data such as Kitchen, Ingredient, Recipe, etc.
    /// </summary>
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