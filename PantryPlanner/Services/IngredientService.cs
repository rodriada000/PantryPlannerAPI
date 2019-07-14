using Microsoft.EntityFrameworkCore;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    public class IngredientService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        public PermissionService Permissions { get; set; }

        public IngredientService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }


        #region Get Methods

        /// <summary>
        /// Return list of Ingredients with names that match the given <paramref name="name"/> passed in.
        /// </summary>
        /// <param name="name"> name to search for </param>
        public List<Ingredient> GetIngredientByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return Context.Ingredient.Where(i => i.Name.Contains(name, StringComparison.OrdinalIgnoreCase) && i.IsPublic).ToList();
        }

        /// <summary>
        /// Return list of Ingredients in the passed in <paramref name="category"/> and with names that match the given <paramref name="name"/> passed in.
        /// </summary>
        public List<Ingredient> GetIngredientByNameAndCategory(string name, Category category)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            return Context.Ingredient.Where(i => i.Name.Contains(name, StringComparison.OrdinalIgnoreCase) && i.CategoryId == category.CategoryId && i.IsPublic).ToList();
        }

        /// <summary>
        /// Return list of Ingredients with names that match the given <paramref name="name"/> passed in.
        /// </summary>
        public List<Ingredient> GetIngredientByNameAndCategory(string name, string categoryName)
        {
            if (String.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            Category foundCategory = (from category in Context.Category
                                      join catType in Context.CategoryType on category.CategoryTypeId equals catType.CategoryTypeId
                                      where catType.Name == "Ingredient" && category.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase)
                                      select category).FirstOrDefault();

            if (foundCategory == null)
            {
                throw new CategoryNotFoundException();
            }


            return GetIngredientByNameAndCategory(name, foundCategory);
        }

        /// <summary>
        /// Return list of ingredients with descriptions that match the given <paramref name="description"/> passed in.
        /// </summary>
        /// <param name="description"> text to search for in Ingredient description </param>
        public List<Ingredient> GetIngredientByDescription(string description)
        {
            if (String.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }


            return Context.Ingredient.Where(i => i.Description.Contains(description, StringComparison.OrdinalIgnoreCase) && i.IsPublic).ToList();
        }

        /// <summary>
        /// Return list of ingredients with in the the given <paramref name="category"/>.
        /// </summary>
        public List<Ingredient> GetIngredientByCategory(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            return GetIngredientByCategory(category.CategoryId);
        }

        /// <summary>
        /// Return list of ingredients with in the the given <paramref name="categoryId"/>.
        /// </summary>
        public List<Ingredient> GetIngredientByCategory(long categoryId)
        {
            if (Context.CategoryExists(categoryId) == false)
            {
                throw new CategoryNotFoundException(categoryId);
            }

            return Context.Ingredient.Where(i => i.CategoryId == categoryId && i.IsPublic).ToList();
        }

        #endregion


        #region Add Methods


        /// <summary>
        /// Adds an ingredient to the <see cref="Context"/> that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newIngredient"> ingredient to add </param>
        /// <param name="user"> user who is adding ingredient </param>
        public void AddIngredient(Ingredient newIngredient, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newIngredient == null)
            {
                throw new ArgumentNullException(nameof(newIngredient));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            // validate an ingredient with same name in the same category doesn't already exist
            if (Context.IngredientExistsForUser(newIngredient, user))
            {
                throw new InvalidOperationException($"An ingredient with the name {newIngredient.Name} already exists");
            }

            newIngredient.AddedByUserId = user.Id;
            newIngredient.DateAdded = DateTime.Now;

            Context.Ingredient.Add(newIngredient);
            Context.SaveChanges();
        }

        /// <summary>
        /// Adds an ingredient marked as IsPublic=true to the <see cref="Context"/>.
        /// The AddedByUserId will be null.
        /// </summary>
        public void AddIngredient(Ingredient newIngredient)
        {
            if (newIngredient == null)
            {
                throw new ArgumentNullException(nameof(newIngredient));
            }

            // validate an ingredient with same name in the same category doesn't already exist
            if (Context.IngredientExistsPublicly(newIngredient))
            {
                throw new InvalidOperationException($"An ingredient with the name {newIngredient.Name} already exists");
            }

            newIngredient.AddedByUserId = null;
            newIngredient.IsPublic = true;
            newIngredient.DateAdded = DateTime.Now;

            Context.Ingredient.Add(newIngredient);
            Context.SaveChanges();
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Updates ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public void UpdateIngredient(Ingredient ingredient, PantryPlannerUser userUpdating)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Permissions.UserAddedIngredient(ingredient, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this ingredient");
            }

            Context.Entry(ingredient).State = EntityState.Modified;
            Context.SaveChanges();
        }

        #endregion


        #region Delete Methods

        /// <summary>
        /// Deletes ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public Ingredient DeleteIngredient(Ingredient ingredient, PantryPlannerUser userDeleting)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            return DeleteIngredient(ingredient.IngredientId, userDeleting);
        }

        /// <summary>
        /// Deletes ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public Ingredient DeleteIngredient(long ingredientId, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (Context.IngredientExists(ingredientId) == false)
            {
                throw new IngredientNotFoundException(ingredientId);
            }

            if (Permissions.UserAddedIngredient(ingredientId, userDeleting) == false)
            {
                throw new PermissionsException($"You do not have rights to delete this ingredient");
            }

            Ingredient ingredientToDelete = Context.Ingredient.Find(ingredientId);

            Context.Ingredient.Remove(ingredientToDelete);
            Context.SaveChanges();

            return ingredientToDelete;
        }

        #endregion

    }
}
