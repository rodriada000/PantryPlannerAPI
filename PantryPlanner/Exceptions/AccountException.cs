using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    /// <summary>
    /// Exception that can be thrown when an Account action such as Login, Register, fails
    /// </summary>
    [Serializable]
    public class AccountException : Exception
    {
        public AccountException() : base("Failed to authorize account.")
        {
        }

        public AccountException(string message) : base(message)
        {
        }

        public AccountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AccountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}