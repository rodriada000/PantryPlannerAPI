using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class RecipeDto
    {
        public long RecipeId { get; set; }
        public string CreatedByUserId { get; set; }
        public string RecipeUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PrepTime { get; set; }
        public int? CookTime { get; set; }
        public string ServingSize { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsPublic { get; set; }


        #region Additional Properties Not In Model

        public string CreatedByUsername { get; set; }
        public List<RecipeIngredientDto> Ingredients { get; set; }
        public List<RecipeStepDto> Steps { get; set; }

        #endregion

        public RecipeDto()
        {
        }

        public RecipeDto(Recipe recipe)
        {
            if (recipe == null)
            {
                return;
            }

            RecipeId = recipe.RecipeId;
            CreatedByUserId = recipe.CreatedByUserId;
            RecipeUrl = recipe.RecipeUrl;
            Name = recipe.Name;
            Description = recipe.Description;
            PrepTime = recipe.PrepTime;
            CookTime = recipe.CookTime;
            ServingSize = recipe.ServingSize;
            DateCreated = recipe.DateCreated;
            IsPublic = recipe.IsPublic;

            CreatedByUsername = recipe.CreatedByUser?.UserName;
            Ingredients = RecipeIngredientDto.ToList(recipe.RecipeIngredient);
            Steps = RecipeStepDto.ToList(recipe.RecipeStep);
        }

        public static List<RecipeDto> ToList(List<Recipe> list)
        {
            return list?.Select(k => new RecipeDto(k))?.ToList();
        }

        public override string ToString()
        {
            string str = $"r: {Name}; pt = {PrepTime} ct = {CookTime} ss = {ServingSize}";

            if (string.IsNullOrWhiteSpace(RecipeUrl) == false)
            {
                str += $" | {RecipeUrl}";
            }

            return str;
        }

        /// <summary>
        /// Return a <see cref="Recipe"/> object based on the DTO
        /// </summary>
        internal Recipe Create()
        {
            return new Recipe()
            {
                RecipeId = this.RecipeId,
                CreatedByUserId = this.CreatedByUserId,
                RecipeUrl = this.RecipeUrl,
                Name = this.Name,
                Description = this.Description,
                PrepTime = this.PrepTime,
                CookTime = this.CookTime,
                ServingSize = this.ServingSize,
                DateCreated = this.DateCreated,
                IsPublic = this.IsPublic
            };
        }
    }
}
