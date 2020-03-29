using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class RecipeStepNotFoundException : Exception
    {
        private long recipeStepId;

        public RecipeStepNotFoundException()
        {
        }

        public RecipeStepNotFoundException(long recipeStepId) : base($"Recipe/Step with ID {recipeStepId} does not exist.")
        {
            this.recipeStepId = recipeStepId;
        }

        public RecipeStepNotFoundException(string message) : base(message)
        {
        }

        public RecipeStepNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecipeStepNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}