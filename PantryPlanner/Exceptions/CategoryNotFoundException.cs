using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class CategoryNotFoundException : Exception
    {
        public long CategoryId { get; set; }

        public CategoryNotFoundException() : base("No category exists")
        {
        }

        public CategoryNotFoundException(long categoryId) : base($"Category with ID {categoryId} does not exist.")
        {
            this.CategoryId = categoryId;
        }

        public CategoryNotFoundException(string message) : base(message)
        {
        }

        public CategoryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CategoryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}