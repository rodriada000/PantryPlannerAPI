using Microsoft.EntityFrameworkCore;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    public class KitchenIngredientService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        public PermissionService Permissions { get; set; }

        public KitchenIngredientService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }


        #region Get Methods

        /// <summary>
        /// Return all ingredients in the <paramref name="kitchen"/>. 
        /// </summary>
        public List<KitchenIngredient> GetKitchenIngredients(long kitchenId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            if (Permissions.UserHasRightsToKitchen(user, kitchenId) == false)
            {
                throw new PermissionsException("You do not have rights to this kitchen");
            }

            return Context.KitchenIngredient.Where(k => k.KitchenId == kitchenId)
                                            .Include(i => i.Ingredient).ThenInclude(i => i.Category)
                                            .Include(i => i.Category)
                                            .Include(i => i.Kitchen)
                                            .Include(i => i.AddedByKitchenUser)
                                            .ToList();
        }

        /// <summary>
        /// Return all ingredients in the <paramref name="kitchen"/>. 
        /// </summary>
        public List<KitchenIngredient> GetKitchenIngredients(Kitchen kitchen, PantryPlannerUser user)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            return GetKitchenIngredients(kitchen.KitchenId, user);
        }

        /// <summary>
        /// Return all ingredients in the <paramref name="kitchen"/> that match the <paramref name="name"/> passed in. 
        /// </summary>
        public List<KitchenIngredient> GetKitchenIngredientsByName(Kitchen kitchen, string name, PantryPlannerUser user)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }


            if (Permissions.UserHasRightsToKitchen(user, kitchen) == false)
            {
                throw new PermissionsException("You do not have rights to this kitchen");
            }

            // first check for exact match
            if (Context.KitchenIngredient.Any(i => i.Ingredient.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return Context.KitchenIngredient.Where(i => i.Ingredient.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Include(i => i.Kitchen)
                                                .Include(i => i.Ingredient).ThenInclude(i => i.Category)
                                                .Include(i => i.Category)
                                                .Include(i => i.AddedByKitchenUser)
                                                .ToList();
            }


            // second check for any matches that have ALL the words entered
            List<string> wordsToSearchFor = name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();

            List<KitchenIngredient> ingredients = Context.KitchenIngredient.Where(i => wordsToSearchFor.All(w => i.Ingredient.Name.Contains(w, StringComparison.OrdinalIgnoreCase)))
                                                                           .Include(i => i.Kitchen)
                                                                           .Include(i => i.Ingredient).ThenInclude(i => i.Category)
                                                                           .Include(i => i.Category)
                                                                           .Include(i => i.AddedByKitchenUser)
                                                                           .ToList();


            if (ingredients.Count == 0)
            {
                // if no matches then lastly check if ANY word entered matches
                ingredients = Context.KitchenIngredient.Where(i => wordsToSearchFor.Any(w => i.Ingredient.Name.Contains(w, StringComparison.OrdinalIgnoreCase)))
                                                       .Include(i => i.Kitchen)
                                                       .Include(i => i.Ingredient).ThenInclude(i => i.Category)
                                                       .Include(i => i.Category)
                                                       .Include(i => i.AddedByKitchenUser)
                                                       .ToList();
            }

            return ingredients;
        }

        /// <summary>
        /// Return all ingredients in the <paramref name="kitchen"/> that match the <paramref name="name"/> passed in. 
        /// </summary>
        public List<KitchenIngredient> GetKitchenIngredientsByName(long kitchenId, string name, PantryPlannerUser user)
        {
            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            Kitchen kitchen = Context.Kitchen.Where(k => k.KitchenId == kitchenId).FirstOrDefault();

            return GetKitchenIngredientsByName(kitchen, name, user);
        }


        /// <summary>
        /// Gets the first Category that matches <paramref name="categoryName"/> and returns all ingredients in the <paramref name="kitchen"/> in the found category.
        /// </summary>
        /// 
        public List<KitchenIngredient> GetKitchenIngredientsByCategoryName(Kitchen kitchen, string categoryName, PantryPlannerUser user)
        {
            Category category = Context.Category.Where(c => c.CategoryType.Name == "KitchenIngredient" && c.Name.Contains(categoryName) && c.CreatedByKitchenId == kitchen.KitchenId)
                                                .Include(c => c.CategoryType)
                                                .Include(c => c.CreatedByKitchen)
                                                .FirstOrDefault();

            if (category == null)
            {
                throw new CategoryNotFoundException($"No category found with the name {categoryName}");
            }

            return GetKitchenIngredientsByCategory(kitchen, category, user);
        }

        /// <summary>
        /// Gets the first Category that matches <paramref name="categoryName"/> and returns all ingredients in the <paramref name="kitchen"/> in the found category.
        /// </summary>
        /// 
        public List<KitchenIngredient> GetKitchenIngredientsByCategoryName(long kitchenId, string categoryName, PantryPlannerUser user)
        {
            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            Kitchen kitchen = Context.Kitchen.Where(k => k.KitchenId == kitchenId).FirstOrDefault();

            return GetKitchenIngredientsByCategoryName(kitchen, categoryName, user);
        }


        /// <summary>
        /// Return all ingredients in the <paramref name="kitchen"/> 
        /// </summary>
        /// 
        public List<KitchenIngredient> GetKitchenIngredientsByCategory(Kitchen kitchen, Category category, PantryPlannerUser user)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Permissions.UserHasRightsToKitchen(user, kitchen) == false)
            {
                throw new PermissionsException("You do not have rights to this kitchen");
            }

            if (Context.CategoryExists(category.CategoryId) == false)
            {
                throw new CategoryNotFoundException(category.CategoryId);
            }

            return Context.KitchenIngredient.Where(k => k.KitchenId == kitchen.KitchenId && k.CategoryId == category.CategoryId)
                                            .Include(i => i.Ingredient).ThenInclude(i => i.Category)
                                            .Include(i => i.Category)
                                            .Include(i => i.AddedByKitchenUser)
                                            .ToList();
        }

        /// <summary>
        /// Return all ingredients in the <paramref name="kitchen"/> that are in a given Category
        /// </summary>
        public List<KitchenIngredient> GetKitchenIngredientsByCategory(long kitchenId, long categoryId, PantryPlannerUser user)
        {
            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            if (Context.CategoryExists(categoryId) == false)
            {
                throw new CategoryNotFoundException(categoryId);
            }


            Kitchen kitchen = Context.Kitchen.Where(k => k.KitchenId == kitchenId).FirstOrDefault();
            Category category = Context.Category.Where(c => c.CategoryId == categoryId).FirstOrDefault();

            return GetKitchenIngredientsByCategory(kitchen, category, user);
        }


        /// <summary>
        /// Return all ingredients in the <paramref name="kitchen"/> that match the <paramref name="name"/> passed in and are in the <paramref name="category"/> passed in. 
        /// </summary>
        public List<KitchenIngredient> GetKitchenIngredientsByNameAndCategoryName(Kitchen kitchen, string name, string categoryName, PantryPlannerUser user)
        {
            Category category = Context.Category.Where(c => c.CategoryType.Name == "KitchenIngredient" && c.Name.Contains(categoryName) && c.CreatedByKitchenId == kitchen.KitchenId)
                                                .Include(c => c.CategoryType)
                                                .Include(c => c.CreatedByKitchen)
                                                .FirstOrDefault();

            if (category == null)
            {
                throw new CategoryNotFoundException($"No category found with the name {categoryName}");
            }

            return GetKitchenIngredientsByNameAndCategory(kitchen, name, category, user);
        }

        /// <summary>
        /// Return all ingredients in the <paramref name="kitchen"/> that match the <paramref name="name"/> passed in and are in the <paramref name="category"/> passed in. 
        /// </summary>
        public List<KitchenIngredient> GetKitchenIngredientsByNameAndCategory(Kitchen kitchen, string name, Category category, PantryPlannerUser user)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (Context.CategoryExists(category.CategoryId) == false)
            {
                throw new CategoryNotFoundException(category.CategoryId);
            }

            return GetKitchenIngredientsByName(kitchen, name, user)
                   .Where(k => k.CategoryId == category.CategoryId)
                   .ToList();
        }


        /// <summary>
        /// Return KitchenIngredient for <paramref name="kitchenIngredientId"/>
        /// </summary>
        public KitchenIngredient GetKitchenIngredientById(long kitchenIngredientId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.KitchenIngredientExists(kitchenIngredientId) == false)
            {
                throw new IngredientNotFoundException($"No KitchenIngredient exists with ID {kitchenIngredientId}");
            }

            KitchenIngredient ingredient = GetKitchenIngredientById(kitchenIngredientId);


            if (Permissions.UserHasRightsToKitchen(user, ingredient.KitchenId) == false)
            {
                throw new PermissionsException("You do not have rights to this ingredient");
            }

            return ingredient;
        }

        /// <summary>
        /// Return Ingredient for <paramref name="kitchenIngredientId"/>
        /// </summary>
        public KitchenIngredient GetKitchenIngredientById(long kitchenIngredientId)
        {
            return Context.KitchenIngredient.Where(i => i.KitchenIngredientId == kitchenIngredientId)
                                                .Include(i => i.Ingredient).ThenInclude(i => i.Category)
                                                .Include(i => i.Category)
                                                .Include(i => i.AddedByKitchenUser)
                                                .Include(i => i.Kitchen)
                                                .FirstOrDefault();
        }

        #endregion


        #region Add Methods


        /// <summary>
        /// Adds an ingredient to the kitchen that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newIngredient"> ingredient to add </param>
        /// <param name="user"> user who is adding ingredient </param>
        public KitchenIngredient AddKitchenIngredient(KitchenIngredient newIngredient, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newIngredient == null)
            {
                throw new ArgumentNullException(nameof(newIngredient));
            }

            return AddIngredientToKitchen(newIngredient.IngredientId, newIngredient.KitchenId, user);
        }

        /// <summary>
        /// Adds an ingredient to the kitchen that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newIngredient"> ingredient to add </param>
        /// <param name="user"> user who is adding ingredient </param>
        public KitchenIngredient AddIngredientToKitchen(Ingredient newIngredient, Kitchen kitchen, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            if (newIngredient == null)
            {
                throw new ArgumentNullException(nameof(newIngredient));
            }

            return AddIngredientToKitchen(newIngredient.IngredientId, kitchen.KitchenId, user);
        }

        /// <summary>
        /// Adds an ingredient to the kitchen that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="ingredientId"> ingredient to add </param>
        /// <param name="user"> user who is adding ingredient </param>
        public KitchenIngredient AddIngredientToKitchen(long ingredientId, long kitchenId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            if (Context.IngredientExists(ingredientId) == false)
            {
                throw new IngredientNotFoundException(ingredientId);
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }


            if (Permissions.UserHasRightsToKitchen(user, kitchenId) == false)
            {
                throw new PermissionsException("You do not have rights to add ingredients to this kitchen");
            }


            if (Context.IngredientExistsForKitchen(ingredientId, kitchenId))
            {
                throw new InvalidOperationException($"This ingredient has already been added.");
            }

            KitchenUser kitchenUser = Context.KitchenUser.Where(u => u.KitchenId == kitchenId && u.UserId == user.Id).FirstOrDefault();

            KitchenIngredient ingredientToAdd = new KitchenIngredient()
            {
                KitchenId = kitchenId,
                IngredientId = ingredientId,
                AddedByKitchenUserId = kitchenUser.KitchenUserId,
                AddedByKitchenUser = kitchenUser,
                LastUpdated = DateTime.Now,
            };

            Context.KitchenIngredient.Add(ingredientToAdd);
            Context.SaveChanges();

            if (ingredientToAdd.Ingredient == null)
            {
                ingredientToAdd.Ingredient = Context.Ingredient.Where(i => i.IngredientId == ingredientId)
                                                               .Include(i => i.Category)
                                                               .Include(i => i.AddedByUser)
                                                               .FirstOrDefault();
            }

            if (ingredientToAdd.Kitchen == null)
            {
                ingredientToAdd.Kitchen = Context.Kitchen.Where(k => k.KitchenId == kitchenId)
                                                         .Include(k => k.CreatedByUser)
                                                         .FirstOrDefault();
            }

            return ingredientToAdd;
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Updates ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public async Task<bool> UpdateKitchenIngredientAsync(KitchenIngredientDto ingredientDto, PantryPlannerUser userUpdating)
        {
            if (ingredientDto == null)
            {
                throw new ArgumentNullException(nameof(ingredientDto));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.KitchenIngredientExists(ingredientDto.KitchenIngredientId) == false)
            {
                throw new IngredientNotFoundException($"KitchenIngredient with ID {ingredientDto.KitchenIngredientId} does not exist.");
            }

            if (Permissions.UserHasRightsToKitchen(userUpdating, ingredientDto.KitchenId) == false)
            {
                throw new PermissionsException("You do not have rights to update this ingredient");
            }



            KitchenIngredient kitchenIngredientToUpdate = Context.KitchenIngredient
                                                                 .Where(ki => ki.KitchenIngredientId == ingredientDto.KitchenIngredientId)
                                                                 .FirstOrDefault();

            // only update fields that are not null in the DTO
            if (ingredientDto.Note != null)
            {
                kitchenIngredientToUpdate.Note = ingredientDto.Note;
            }

            if (ingredientDto.Quantity != null)
            {
                kitchenIngredientToUpdate.Quantity = ingredientDto.Quantity;
            }

            kitchenIngredientToUpdate.LastUpdated = DateTime.UtcNow;

            Context.Entry(kitchenIngredientToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Updates ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public async Task<bool> UpdateKitchenIngredientAsync(KitchenIngredient ingredient, PantryPlannerUser userUpdating)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.KitchenIngredientExists(ingredient) == false)
            {
                throw new IngredientNotFoundException($"KitchenIngredient with ID {ingredient.KitchenIngredientId} does not exist.");
            }

            if (Permissions.UserHasRightsToKitchen(userUpdating, ingredient.KitchenId) == false)
            {
                throw new PermissionsException("You do not have rights to update this ingredient");
            }

            Context.Entry(ingredient).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        #endregion


        #region Delete Methods

        /// <summary>
        /// Deletes ingredient if user has rights to it (i.e. user has rights to kitchen)
        /// </summary>
        public KitchenIngredient DeleteKitchenIngredient(KitchenIngredient ingredient, PantryPlannerUser userDeleting)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            return DeleteKitchenIngredient(ingredient.KitchenIngredientId, userDeleting);
        }

        /// <summary>
        /// Deletes ingredient if user has rights to it (i.e. user has rights to kitchen)
        /// </summary>
        public KitchenIngredient DeleteKitchenIngredient(long kitchenIngredientId, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (Context.KitchenIngredientExists(kitchenIngredientId) == false)
            {
                throw new IngredientNotFoundException($"KitchenIngredient with ID {kitchenIngredientId} does not exist.");
            }

            KitchenIngredient ingredientToDelete = Context.KitchenIngredient.Where(k => k.KitchenIngredientId == kitchenIngredientId).FirstOrDefault();

            if (Permissions.UserHasRightsToKitchen(userDeleting, ingredientToDelete.KitchenId) == false)
            {
                throw new PermissionsException($"You do not have rights to delete this ingredient");
            }

            Context.KitchenIngredient.Remove(ingredientToDelete);
            Context.SaveChanges();

            return ingredientToDelete;
        }

        #endregion

    }
}
