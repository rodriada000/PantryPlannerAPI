using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class RecipeIngredientNotFoundException : Exception
    {
        private long recipeIngredientId;

        public RecipeIngredientNotFoundException()
        {
        }

        public RecipeIngredientNotFoundException(long recipeIngredientId) : base($"Recipe/Ingredient with ID {recipeIngredientId} does not exist.")
        {
            this.recipeIngredientId = recipeIngredientId;
        }

        public RecipeIngredientNotFoundException(string message) : base(message)
        {
        }

        public RecipeIngredientNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecipeIngredientNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}