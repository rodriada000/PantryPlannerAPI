using PantryPlanner.Classes;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using System;
using System.Linq;
using Xunit;

namespace PantryPlannerApiUnitTests
{
    public class FoodCompositionETLUnitTest
    {
        PantryPlannerContext _context;
        USDAFoodCompositionDbETL _etl;

        /// <summary>
        /// Folder location of the .txt files with test data
        /// </summary>
        public static string FoodCompositionFolderLocation = ".\\TestData";

        public FoodCompositionETLUnitTest()
        {
            _context = InMemoryDataGenerator.CreateInMemoryDatabaseContext(Guid.NewGuid().ToString());
            _etl = new USDAFoodCompositionDbETL(FoodCompositionFolderLocation);
        }

        [Fact]
        public void StartEtlProcess_InsertsCorrectResults()
        {
            // arrange: run the ETL process once
            int expectedCount = 2500;
            _etl.StartEtlProcess(_context, expectedCount);

            // assert: correct amount of records inserted based on what it is in test files
            Assert.Equal(expectedCount, _context.Ingredient.Count());

            int expectedFoodGroupCount = 25;
            Assert.Equal(expectedFoodGroupCount, _context.Category.Count());
        }


        [Fact]
        public void StartEtlProcess_SkipsDuplicates()
        {
            // arrange: run the ETL process once
            int expectedCount = 100;

            _etl.StartEtlProcess(_context, expectedCount);

            int expectedFoodGroupCount = _context.Category.Count();
            
            // act: running the ETL process again should run successfully without inserting duplicates
            _etl.StartEtlProcess(_context, expectedCount);


            // assert: nothing new was inserted
            Assert.Equal(expectedCount, _context.Ingredient.Count());
            Assert.Equal(expectedFoodGroupCount, _context.Category.Count());
        }

    }
}
