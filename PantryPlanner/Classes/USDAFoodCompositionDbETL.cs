using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Classes
{
    /// <summary>
    /// This class is used to run a ETL process on the USDA Food Composition Database .txt files
    /// to load data into <see cref="Ingredient"/> and <see cref="Category"/> Tables
    /// </summary>
    /// <remarks>
    /// The database data is provided via ASCII .txt files with a specific format. The USDA provides a PDF
    /// that explains the format and contents of each .txt file here: https://www.ars.usda.gov/ARSUserFiles/80400525/Data/SR-Legacy/SR-Legacy_Doc.pdf
    /// </remarks>
    public class USDAFoodCompositionDbETL
    {
        /// <summary>
        /// Root folder that contains the .txt files
        /// </summary>
        public string TxtRootFolderPath { get; set; }

        /// <summary>
        /// Mapping of Food Group Code used by USDA to the CategoryID used in PantryPlanner system
        /// </summary>
        private Dictionary<string, long> FoodGroupCodeToCategoryId { get; set; }

        /// <summary>
        /// Name of file with food group data
        /// </summary>
        public const string FoodGroupFileName = "FD_GROUP.txt";

        /// <summary>
        /// Name of file with food description data
        /// </summary>
        public const string FoodDescriptionFileName = "FOOD_DES.txt";

        /// <summary>
        /// Mapping of the food description column name to the array index
        /// </summary>
        private Dictionary<string, int> _foodDescColumnNameToIndex;

        /// <summary>
        /// Mapping of the food description column name to the array index
        /// </summary>
        public Dictionary<string, int> FoodDescColumnNameToIndex
        {
            get
            {
                if (_foodDescColumnNameToIndex != null)
                {
                    return _foodDescColumnNameToIndex;
                }

                _foodDescColumnNameToIndex = new Dictionary<string, int>();
                _foodDescColumnNameToIndex.TryAdd("NDBID", 0);
                _foodDescColumnNameToIndex.TryAdd("FoodGrpCode", 1);
                _foodDescColumnNameToIndex.TryAdd("LongDesc", 2);
                _foodDescColumnNameToIndex.TryAdd("ShortDesc", 3);
                _foodDescColumnNameToIndex.TryAdd("ComName", 4);
                _foodDescColumnNameToIndex.TryAdd("ManufacName", 5);
                _foodDescColumnNameToIndex.TryAdd("Survey", 6);
                _foodDescColumnNameToIndex.TryAdd("Ref_desc", 7);
                _foodDescColumnNameToIndex.TryAdd("Refuse", 8);
                _foodDescColumnNameToIndex.TryAdd("SciName", 9);
                _foodDescColumnNameToIndex.TryAdd("N_Factor", 10);
                _foodDescColumnNameToIndex.TryAdd("Pro_Factor", 11);
                _foodDescColumnNameToIndex.TryAdd("Fat_Factor", 12);
                _foodDescColumnNameToIndex.TryAdd("CHO_Factor", 13);

                return _foodDescColumnNameToIndex;
            }
        }


        public USDAFoodCompositionDbETL(string folderPathToDbSource)
        {
            TxtRootFolderPath = folderPathToDbSource;

            if (TxtRootFolderPath.EndsWith("\\") == false)
            {
                TxtRootFolderPath = TxtRootFolderPath + "\\";
            }

            FoodGroupCodeToCategoryId = new Dictionary<string, long>();
        }

        /// <summary>
        /// runs the two main methods to load and parse the .txt data into the Ingredient/Category tables.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="maxRowsToProcess"> -1 to process all; otherwise parse/insert ingredients upto the max</param>
        public void StartEtlProcess(PantryPlannerContext context, int maxRowsToProcess = -1)
        {
            ParseFoodGroupFileAndInsertIntoCategoryTable(context);
            ParseFoodDescriptionFileAndInsertIntoIngredientTable(context, maxRowsToProcess);
        }

        private void ParseFoodDescriptionFileAndInsertIntoIngredientTable(PantryPlannerContext context, int maxRowsToProcess = -1)
        {
            int lineCount = 0;
            List<Ingredient> ingredientsToAdd = new List<Ingredient>();

            // validate file exists
            string fullPath = TxtRootFolderPath + FoodDescriptionFileName;

            if (File.Exists(fullPath) == false)
            {
                throw new FileNotFoundException($"Could not find {FoodDescriptionFileName} at the specified path", fullPath);
            }


            foreach (string line in File.ReadAllLines(fullPath))
            {
                if (maxRowsToProcess != -1 && lineCount >= maxRowsToProcess)
                {
                    break; // stop parsing after reaching the max row count passed in
                }

                lineCount++;

                // parse line by format rules (each field is seperated by ^)
                List<string> fieldValues = line.Split("^").ToList();

                string nutrientDBID = fieldValues[FoodDescColumnNameToIndex["NDBID"]].Trim('~');
                string foodGroupCode = fieldValues[FoodDescColumnNameToIndex["FoodGrpCode"]].Trim('~');
                string longDescription = fieldValues[FoodDescColumnNameToIndex["LongDesc"]].Trim('~');
                string commonNames = fieldValues[FoodDescColumnNameToIndex["ComName"]].Trim('~');

                FoodGroupCodeToCategoryId.TryGetValue(foodGroupCode, out long foodGroupCategoryId);

                if (foodGroupCategoryId == 0)
                {
                    continue; // unknown food category, lets skip
                }


                bool categoryExists = context.Ingredient.Any(i => i.CategoryId == foodGroupCategoryId && i.Name == longDescription);

                if (categoryExists)
                {
                    continue;
                }

                Ingredient newIngredient = new Ingredient()
                {
                    Name = longDescription,
                    CategoryId = foodGroupCategoryId,
                    IsPublic = true,
                    DateAdded = DateTime.Now,
                    Description = ""
                };

                ingredientsToAdd.Add(newIngredient);
            }

            if (ingredientsToAdd.Count > 0)
            {
                context.Ingredient.AddRange(ingredientsToAdd);
                context.SaveChanges();
            }

        }

        private void ParseFoodGroupFileAndInsertIntoCategoryTable(PantryPlannerContext context)
        {
            // validate file exists
            string fullPath = TxtRootFolderPath + FoodGroupFileName;

            if (File.Exists(fullPath) == false)
            {
                throw new FileNotFoundException($"Could not find {FoodGroupFileName} at the specified path", fullPath);
            }


            // ensure CategoryType 'Ingredient' exists
            CategoryType ingredientCategoryType;

            if (context.CategoryType.Any(c => c.Name == "Ingredient") == false)
            {
                ingredientCategoryType = new CategoryType()
                {
                    Name = "Ingredient"
                };

                context.CategoryType.Add(ingredientCategoryType);
                context.SaveChanges();
            }
            else
            {
                ingredientCategoryType = context.CategoryType.Where(c => c.Name == "Ingredient").FirstOrDefault();
            }

            List<Category> categoriesToAdd = new List<Category>();

            foreach (string line in File.ReadAllLines(fullPath))
            {
                // parse line by format rules (each field is seperated by ^)
                List<string> fieldValues = line.Split("^").ToList();

                // get code and food group name (trim ~ based on format rules)
                string code = fieldValues[0].Trim('~');
                string foodGroupDesc = fieldValues[1].Trim('~');

                // check if food group name already exists in context
                bool categoryExists = context.Category.Any(c => c.Name == foodGroupDesc && c.CategoryTypeId == ingredientCategoryType.CategoryTypeId);

                if (categoryExists)
                {
                    continue;
                }

                // add food group as an Ingredient category
                Category newFoodGroup = new Category()
                {
                    CategoryTypeId = ingredientCategoryType.CategoryTypeId,
                    Name = foodGroupDesc
                };

                categoriesToAdd.Add(newFoodGroup);
            }

            if (categoriesToAdd.Count > 0)
            {
                context.Category.AddRange(categoriesToAdd);
                context.SaveChanges();
            }


            // Map out IDs to codes for later inserting ingredients
            foreach (string line in File.ReadAllLines(fullPath))
            {
                // parse line by format rules (each field is seperated by ^)
                List<string> fieldValues = line.Split("^").ToList();

                // get code and food group name (trim ~ based on format rules)
                string code = fieldValues[0].Trim('~');
                string foodGroupDesc = fieldValues[1].Trim('~');

                Category existingCategory = context.Category.Where(c => c.Name == foodGroupDesc && c.CategoryTypeId == ingredientCategoryType.CategoryTypeId).FirstOrDefault();

                if (existingCategory != null)
                {
                    FoodGroupCodeToCategoryId.TryAdd(code, existingCategory.CategoryId);
                }
            }

        }

    }
}
