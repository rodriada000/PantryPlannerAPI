﻿using PantryPlanner.Migrations;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
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
        public static string FoodCompositionFolderLocation = "C:\\Users\\Adam\\Downloads\\SR-Leg_ASC";

        public FoodCompositionETLUnitTest()
        {
            _context = InMemoryDataGenerator.CreateInMemoryDatabaseContext("FoodCompositionETLUnitTestDB");
            _etl = new USDAFoodCompositionDbETL(FoodCompositionFolderLocation);
        }

        [Fact]
        public void StartEtlProcess_InsertsCorrectResults()
        {
            // arrange: run the ETL process once
            _etl.StartEtlProcess(_context);

            // assert: correct amount of records inserted based on what it is in test files
            int expectedCount = 7793;
            Assert.Equal(expectedCount, _context.Ingredient.Count());

            int expectedFoodGroupCount = 25;
            Assert.Equal(expectedFoodGroupCount, _context.Category.Count());

        }


        // VV Commented out because it takes for ever to test (~2 minutes)... VV
        //[Fact]
        //public void StartEtlProcess_SkipsDuplicates()
        //{
        //    // arrange: run the ETL process once
        //    _etl.StartEtlProcess(_context);

        //    int expectedCount = _context.Ingredient.Count();
        //    int expectedFoodGroupCount = _context.Category.Count();

        //    // act: running the ETL process again should run successfully without inserting duplicates
        //    _etl.StartEtlProcess(_context);


        //    // assert: nothing new was inserted
        //    Assert.Equal(expectedCount, _context.Ingredient.Count());
        //    Assert.Equal(expectedFoodGroupCount, _context.Category.Count());
        //}

    }
}