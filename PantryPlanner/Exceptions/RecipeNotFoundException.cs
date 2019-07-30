using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class RecipeNotFoundException : Exception
    {
        private long recipeId;

        public RecipeNotFoundException()
        {
        }

        public RecipeNotFoundException(long recipeId) : base($"Recipe with ID {recipeId} does not exist.")
        {
            this.recipeId = recipeId;
        }

        public RecipeNotFoundException(string message) : base(message)
        {
        }

        public RecipeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecipeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}