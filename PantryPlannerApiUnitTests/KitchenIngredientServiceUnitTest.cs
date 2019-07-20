﻿using PantryPlanner.Exceptions;
using PantryPlanner.Migrations;
using PantryPlanner.Models;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PantryPlannerApiUnitTests
{
    public class KitchenIngredientServiceUnitTest
    {
        PantryPlannerContext _context;
        KitchenIngredientService _service;
        PantryPlannerUser _testUser;

        public KitchenIngredientServiceUnitTest()
        {
            _testUser = InMemoryDataGenerator.TestUser;
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser, insertIngredientData: true);


            _service = new KitchenIngredientService(_context);
        }


        #region Get Test Methods

        [Fact]
        public void GetKitchenIngredients_NullArguments_ThrowsArgumentNullException()
        {
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is not setup for testing");
            }

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.GetKitchenIngredients(null, _testUser);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.GetKitchenIngredients(kitchen, null);
            });
        }

        [Fact]
        public void GetKitchenIngredients_UserWithNoRights_ThrowsPermissionException()
        {
            List<long> myKitchenIds = _testUser.KitchenUser.Select(ku => ku.KitchenId).ToList();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            // find a user by getting KitchenUsers that are not in any Kitchen of _testUser
            PantryPlannerUser userWithNoRights = _context.KitchenUser
                                                         .Where(u => u.UserId != _testUser.Id && myKitchenIds.Contains(u.KitchenId) == false) 
                                                         .FirstOrDefault()?.User;

            if (kitchen == null || userWithNoRights == null)
            {
                throw new ArgumentNullException("kitchen or user is not setup for testing");
            }

            Assert.Throws<PermissionsException>(() =>
            {
                _service.GetKitchenIngredients(kitchen, userWithNoRights);
            });

        }

        [Fact]
        public void GetKitchenIngredients_ValidKitchenAndUser_ReturnsCorrectResult()
        {
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is not setup for testing");
            }

            List<KitchenIngredient> expectedIngredients = _context.KitchenIngredient.Where(i => i.KitchenId == kitchen.KitchenId).ToList();


            List<KitchenIngredient> ingredients = _service.GetKitchenIngredients(kitchen, _testUser);

            Assert.Equal(expectedIngredients, ingredients);
        }

        [Fact]
        public void GetKitchenIngredientsByName_ValidKitchenAndUser_ReturnsCorrectResult()
        {
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is not setup for testing");
            }

            // add two new ingredients to the database
            Ingredient testIngredient = new Ingredient()
            {
                AddedByUserId = _testUser.Id,
                Name = "test ingredient 1"
            };

            Ingredient testIngredient2 = new Ingredient()
            {
                AddedByUserId = _testUser.Id,
                Name = "test ingredient 2"
            };

            _context.Ingredient.Add(testIngredient);
            _context.Ingredient.Add(testIngredient2);
            _context.SaveChanges();

            // make sure the new ingredients are in the kitchen  
            _service.AddIngredientToKitchen(testIngredient, kitchen, _testUser);
            _service.AddIngredientToKitchen(testIngredient2, kitchen, _testUser);

 
            List<KitchenIngredient> ingredients = _service.GetKitchenIngredientsByName(kitchen, "test ingredient", _testUser);

            
            Assert.Equal(2, ingredients.Count);
        }

        #endregion


        #region Delete Test Methods

        [Fact]
        public void DeleteKitchenIngredient_NullArguments_ThrowsArgumentNullException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }

            KitchenIngredient addedIngredient = _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.DeleteKitchenIngredient(null, _testUser);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.DeleteKitchenIngredient(addedIngredient, null);
            });
        }

        [Fact]
        public void DeleteKitchenIngredient_InvalidKitchenIngredient_ThrowsIngredientNotFoundException()
        {
            Assert.Throws<IngredientNotFoundException>(() =>
            {
                _service.DeleteKitchenIngredient(new KitchenIngredient() { KitchenIngredientId = -5 }, _testUser);
            });
        }

        [Fact]
        public void DeleteKitchenIngredient_UserNoRights_ThrowsPermissionsException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;
            PantryPlannerUser otherUserTryingToDelete = _context.Users.Where(u => u.Id != _testUser.Id).FirstOrDefault();


            if (ingredient == null || kitchen == null || otherUserTryingToDelete == null)
            {
                throw new ArgumentNullException("ingredient, kitchen, or user is not setup for testing");
            }

            KitchenIngredient addedIngredient = _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);

            Assert.Throws<PermissionsException>(() =>
            {
                _service.DeleteKitchenIngredient(addedIngredient, otherUserTryingToDelete);
            });
        }


        [Fact]
        public void DeleteKitchenIngredient_Valid_ReturnsKitchenIngredientDeleted()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;


            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient, kitchen is not setup for testing");
            }

            KitchenIngredient addedIngredient = _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);

            int countBeforeDelete = kitchen.KitchenIngredient.Count;

            var deletedIngredient = _service.DeleteKitchenIngredient(addedIngredient, _testUser);

            Assert.Equal(addedIngredient, deletedIngredient);
            Assert.Equal(countBeforeDelete - 1, kitchen.KitchenIngredient.Count);
        }


        #endregion


        #region Add Test Methods

        [Fact]
        public void AddKitchenIngredient_ValidIngredientAndUser_ContextIsUpdated()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }

            KitchenIngredient ingredientToAdd = new KitchenIngredient()
            {
                KitchenId = kitchen.KitchenId,
                IngredientId = ingredient.IngredientId,
            };

            int countBeforeAdd = kitchen.KitchenIngredient.Count;

            _service.AddKitchenIngredient(ingredientToAdd, _testUser);

            Assert.Equal(countBeforeAdd + 1, kitchen.KitchenIngredient.Count);
            Assert.NotNull(_context.KitchenIngredient.Where(i => i.KitchenIngredientId == ingredientToAdd.KitchenIngredientId).FirstOrDefault());
        }

        [Fact]
        public void AddKitchenIngredient_AddDuplicateIngredient_ThrowsInvalidOperationException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }

            KitchenIngredient ingredientToAdd = new KitchenIngredient()
            {
                KitchenId = kitchen.KitchenId,
                IngredientId = ingredient.IngredientId,
            };

            _service.AddKitchenIngredient(ingredientToAdd, _testUser);


            Assert.Throws<InvalidOperationException>(() =>
            {
                KitchenIngredient duplicateIngredient = new KitchenIngredient()
                {
                    KitchenId = kitchen.KitchenId,
                    IngredientId = ingredient.IngredientId,
                };

                _service.AddKitchenIngredient(duplicateIngredient, _testUser);
            });

        }

        [Fact]
        public void AddKitchenIngredient_NullUserOrIngredient_ThrowsArgumentNullException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }

            KitchenIngredient ingredientToAdd = new KitchenIngredient()
            {
                KitchenId = kitchen.KitchenId,
                IngredientId = ingredient.IngredientId,
            };


            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.AddKitchenIngredient(null, _testUser);
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.AddKitchenIngredient(ingredientToAdd, null);
            });

        }

        [Fact]
        public void AddKitchenIngredient_UserNoRights_ThrowsPermissionsException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;
            PantryPlannerUser otherUserTryingToAdd = _context.Users.Where(u => u.Id != _testUser.Id).FirstOrDefault();

            if (ingredient == null || kitchen == null || otherUserTryingToAdd == null)
            {
                throw new ArgumentNullException("ingredient, kitchen, or user is not setup for testing");
            }

            KitchenIngredient ingredientToAdd = new KitchenIngredient()
            {
                KitchenId = kitchen.KitchenId,
                IngredientId = ingredient.IngredientId,
            };


            Assert.Throws<PermissionsException>(() =>
            {
                _service.AddKitchenIngredient(ingredientToAdd, otherUserTryingToAdd);
            });

        }

        [Fact]
        public void AddKitchenIngredient_InvalidUser_ThrowsUserNotFoundException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }

            KitchenIngredient ingredientToAdd = new KitchenIngredient()
            {
                KitchenId = kitchen.KitchenId,
                IngredientId = ingredient.IngredientId,
            };


            Assert.Throws<UserNotFoundException>(() =>
            {
                _service.AddKitchenIngredient(ingredientToAdd, new PantryPlannerUser { Id = "fakeuser" });
            });

        }


        [Fact]
        public void AddIngredientToKitchen_ValidIngredientAndUser_ContextIsUpdated()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }

            int countBeforeAdd = kitchen.KitchenIngredient.Count;

            KitchenIngredient addedIngredient = _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);

            Assert.Equal(countBeforeAdd + 1, kitchen.KitchenIngredient.Count);
            Assert.NotNull(_context.KitchenIngredient.Where(i => i.KitchenIngredientId == addedIngredient.KitchenIngredientId).FirstOrDefault());
        }

        [Fact]
        public void AddIngredientToKitchen_AddDuplicateIngredient_ThrowsInvalidOperationException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }


            _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);


            Assert.Throws<InvalidOperationException>(() =>
            {
                _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);
            });

        }

        [Fact]
        public void AddIngredientToKitchen_NullUserOrIngredient_ThrowsArgumentNullException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }


            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.AddIngredientToKitchen(null, kitchen, _testUser);
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.AddIngredientToKitchen(ingredient, null, _testUser);
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.AddIngredientToKitchen(ingredient, kitchen, null);
            });

        }

        [Fact]
        public void AddIngredientToKitchen_InvalidUser_ThrowsUserNotFoundException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }


            Assert.Throws<UserNotFoundException>(() =>
            {
                _service.AddIngredientToKitchen(ingredient.IngredientId, kitchen.KitchenId, new PantryPlannerUser { Id = "fakeuser" });
            });

        }

        [Fact]
        public void AddIngredientToKitchen_UserNoRights_ThrowsPermissionsException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;
            PantryPlannerUser otherUserTryingToAdd = _context.Users.Where(u => u.Id != _testUser.Id).FirstOrDefault();

            if (ingredient == null || kitchen == null || otherUserTryingToAdd == null)
            {
                throw new ArgumentNullException("ingredient, kitchen, or user is not setup for testing");
            }

            Assert.Throws<PermissionsException>(() =>
            {
                _service.AddIngredientToKitchen(ingredient, kitchen, otherUserTryingToAdd);
            });
        }

        [Fact]
        public void AddIngredientToKitchen_InvalidIngredient_ThrowsIngredientNotFoundException()
        {
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is not setup for testing");
            }

            Assert.Throws<IngredientNotFoundException>(() =>
            {
                _service.AddIngredientToKitchen(new Ingredient() { IngredientId = -5 }, kitchen, _testUser);
            });
        }


        [Fact]
        public void AddIngredientToKitchen_InvalidKitchen_ThrowsKitchenNotFoundException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();

            if (ingredient == null)
            {
                throw new ArgumentNullException("ingredient not setup for testing");
            }

            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _service.AddIngredientToKitchen(ingredient, new Kitchen { KitchenId = -5 }, _testUser);
            });
        }

        #endregion


        #region Update Test Methods

        [Fact]
        public void UpdateKitchenIngredient_Valid_ContextIsUpdated()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }

            KitchenIngredient addedIngredient = _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);

            string expectedNote = "hello world";
            int expectedQty = 2;

            addedIngredient.Note = expectedNote;
            addedIngredient.Quantity = expectedQty;

            _service.UpdateKitchenIngredient(addedIngredient, _testUser);

            Assert.Equal(expectedQty, addedIngredient.Quantity);
            Assert.Equal(expectedNote, addedIngredient.Note);
        }

        [Fact]
        public void UpdateKitchenIngredient_UserNoRights_ThrowsPermissionsException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;
            PantryPlannerUser otherUserTryingToUpdate = _context.Users.Where(u => u.Id != _testUser.Id).FirstOrDefault();


            if (ingredient == null || kitchen == null || otherUserTryingToUpdate == null)
            {
                throw new ArgumentNullException("ingredient, kitchen, or user is not setup for testing");
            }

            KitchenIngredient addedIngredient = _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);

            Assert.Throws<PermissionsException>(() =>
            {
                _service.UpdateKitchenIngredient(addedIngredient, otherUserTryingToUpdate);
            });
        }


        [Fact]
        public void UpdateKitchenIngredient_NullArguments_ThrowsArgumentNullException()
        {
            Ingredient ingredient = _context.Ingredient.Where(i => i.Name.Contains("butter")).FirstOrDefault();
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (ingredient == null || kitchen == null)
            {
                throw new ArgumentNullException("ingredient or kitchen is not setup for testing");
            }

            KitchenIngredient addedIngredient = _service.AddIngredientToKitchen(ingredient, kitchen, _testUser);

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.UpdateKitchenIngredient(null, _testUser);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.UpdateKitchenIngredient(addedIngredient, null);
            });
        }


        #endregion

    }
}