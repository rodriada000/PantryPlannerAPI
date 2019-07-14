using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class IngredientNotFoundException : Exception
    {
        private long ingredientId;

        public IngredientNotFoundException()
        {
        }

        public IngredientNotFoundException(long ingredientId) : base($"Ingredient with ID {ingredientId} does not exist.")
        {
            this.ingredientId = ingredientId;
        }

        public IngredientNotFoundException(string message) : base(message)
        {
        }

        public IngredientNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IngredientNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}