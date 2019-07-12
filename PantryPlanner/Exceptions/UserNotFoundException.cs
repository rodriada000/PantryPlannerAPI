using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("No user found")
        {

        }

        public UserNotFoundException(string username) : base($"No user found with the username {username}")
        {

        }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}