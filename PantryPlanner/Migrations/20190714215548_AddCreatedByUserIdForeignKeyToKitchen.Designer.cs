﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PantryPlanner.Services;

namespace PantryPlanner.Migrations
{
    [DbContext(typeof(PantryPlannerContext))]
    [Migration("20190714215548_AddCreatedByUserIdForeignKeyToKitchen")]
    partial class AddCreatedByUserIdForeignKeyToKitchen
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("PantryPlanner.Models.Category", b =>
                {
                    b.Property<long>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CategoryID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CategoryTypeId")
                        .HasColumnName("CategoryTypeID");

                    b.Property<long?>("CreatedByKitchenId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("CategoryId");

                    b.HasIndex("CategoryTypeId")
                        .HasName("fkIdx_161");

                    b.HasIndex("CreatedByKitchenId");

                    b.ToTable("Category","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.CategoryType", b =>
                {
                    b.Property<int>("CategoryTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CategoryTypeID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("CategoryTypeId");

                    b.ToTable("CategoryType","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.Ingredient", b =>
                {
                    b.Property<long>("IngredientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IngredientID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AddedByUserId")
                        .HasColumnName("AddedByUserID");

                    b.Property<long?>("CategoryId")
                        .HasColumnName("CategoryId");

                    b.Property<DateTime>("DateAdded")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Description")
                        .HasMaxLength(100);

                    b.Property<bool>("IsPublic");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte[]>("PreviewPicture")
                        .HasColumnType("image");

                    b.HasKey("IngredientId");

                    b.HasIndex("AddedByUserId")
                        .HasName("fkIdx_40");

                    b.HasIndex("CategoryId");

                    b.ToTable("Ingredient","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.IngredientTag", b =>
                {
                    b.Property<long>("IngredientTagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("IngredientTagID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CreatedByKitchenUserId")
                        .HasColumnName("CreatedByKitchenUserID");

                    b.Property<long>("IngredientId")
                        .HasColumnName("IngredientID");

                    b.Property<long>("KitchenId")
                        .HasColumnName("KitchenID");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("IngredientTagId");

                    b.HasIndex("CreatedByKitchenUserId")
                        .HasName("fkIdx_204");

                    b.HasIndex("IngredientId")
                        .HasName("fkIdx_198");

                    b.HasIndex("KitchenId")
                        .HasName("fkIdx_201");

                    b.ToTable("IngredientTag","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.Kitchen", b =>
                {
                    b.Property<long>("KitchenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("KitchenID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedByUserId")
                        .HasMaxLength(450);

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Description")
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<Guid>("UniquePublicGuid");

                    b.HasKey("KitchenId");

                    b.HasIndex("CreatedByUserId");

                    b.ToTable("Kitchen","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenIngredient", b =>
                {
                    b.Property<long>("KitchenIngredientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("KitchenIngredientID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("IngredientId")
                        .HasColumnName("IngredientID");

                    b.Property<long>("KitchenId")
                        .HasColumnName("KitchenID");

                    b.Property<long?>("AddedByKitchenUserId")
                        .HasColumnName("AddedByKitchenUserID");

                    b.Property<long?>("CategoryId")
                        .HasColumnName("CategoryID");

                    b.Property<DateTime>("LastUpdated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("Quantity");

                    b.HasKey("KitchenIngredientId", "IngredientId", "KitchenId");

                    b.HasIndex("AddedByKitchenUserId")
                        .HasName("fkIdx_115");

                    b.HasIndex("CategoryId")
                        .HasName("fkIdx_187");

                    b.HasIndex("IngredientId")
                        .HasName("fkIdx_50");

                    b.HasIndex("KitchenId")
                        .HasName("fkIdx_47");

                    b.ToTable("KitchenIngredient","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenList", b =>
                {
                    b.Property<long>("KitchenListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("KitchenListID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CategoryId")
                        .HasColumnName("CategoryID");

                    b.Property<long>("KitchenId")
                        .HasColumnName("KitchenID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("KitchenListId");

                    b.HasIndex("CategoryId")
                        .HasName("fkIdx_175");

                    b.HasIndex("KitchenId")
                        .HasName("fkIdx_145");

                    b.ToTable("KitchenList","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenListIngredient", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("ID");

                    b.Property<long>("KitchenListId")
                        .HasColumnName("KitchenListID");

                    b.Property<long>("IngredientId")
                        .HasColumnName("IngredientID");

                    b.Property<long?>("AddedFromRecipeId");

                    b.Property<bool>("IsChecked");

                    b.Property<int?>("Quantity");

                    b.Property<int>("SortOrder");

                    b.HasKey("Id", "KitchenListId", "IngredientId")
                        .HasName("PK_KitchenListRecipe");

                    b.HasIndex("AddedFromRecipeId");

                    b.HasIndex("IngredientId")
                        .HasName("fkIdx_172");

                    b.HasIndex("KitchenListId")
                        .HasName("fkIdx_178");

                    b.ToTable("KitchenListIngredient","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenRecipe", b =>
                {
                    b.Property<long>("KitchenRecipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("KitchenRecipeID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("KitchenId")
                        .HasColumnName("KitchenID");

                    b.Property<long>("RecipeId")
                        .HasColumnName("RecipeID");

                    b.Property<long?>("CategoryId")
                        .HasColumnName("CategoryID");

                    b.Property<bool>("IsFavorite");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false);

                    b.HasKey("KitchenRecipeId", "KitchenId", "RecipeId");

                    b.HasIndex("CategoryId")
                        .HasName("fkIdx_190");

                    b.HasIndex("KitchenId")
                        .HasName("fkIdx_97");

                    b.HasIndex("RecipeId")
                        .HasName("fkIdx_100");

                    b.ToTable("KitchenRecipe","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenUser", b =>
                {
                    b.Property<long>("KitchenUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("KitchenUserID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateAdded");

                    b.Property<bool?>("HasAcceptedInvite")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("((0))");

                    b.Property<bool>("IsOwner");

                    b.Property<long>("KitchenId")
                        .HasColumnName("KitchenID");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnName("UserID");

                    b.HasKey("KitchenUserId");

                    b.HasIndex("KitchenId")
                        .HasName("fkIdx_22");

                    b.HasIndex("UserId")
                        .HasName("fkIdx_19");

                    b.ToTable("KitchenUser","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.MealPlan", b =>
                {
                    b.Property<long>("MealPlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MealPlanID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CategoryId")
                        .HasColumnName("CategoryID");

                    b.Property<long?>("CreatedByKitchenUserId")
                        .HasColumnName("CreatedByKitchenUserID");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Description")
                        .HasMaxLength(255);

                    b.Property<bool>("IsFavorite");

                    b.Property<long>("KitchenId")
                        .HasColumnName("KitchenID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int>("SortOrder");

                    b.HasKey("MealPlanId");

                    b.HasIndex("CategoryId")
                        .HasName("fkIdx_181");

                    b.HasIndex("CreatedByKitchenUserId")
                        .HasName("fkIdx_123");

                    b.HasIndex("KitchenId")
                        .HasName("fkIdx_108");

                    b.ToTable("MealPlan","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.MealPlanRecipe", b =>
                {
                    b.Property<int>("MealPlanRecipeId")
                        .HasColumnName("MealPlanRecipeID");

                    b.Property<long>("RecipeId")
                        .HasColumnName("RecipeID");

                    b.Property<long>("MealPlanId")
                        .HasColumnName("MealPlanID");

                    b.Property<int>("SortOrder");

                    b.HasKey("MealPlanRecipeId", "RecipeId", "MealPlanId");

                    b.HasIndex("MealPlanId")
                        .HasName("fkIdx_136");

                    b.HasIndex("RecipeId")
                        .HasName("fkIdx_133");

                    b.ToTable("MealPlanRecipe","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.PantryPlannerUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("PantryPlanner.Models.Recipe", b =>
                {
                    b.Property<long>("RecipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RecipeID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CookTime");

                    b.Property<string>("CreatedByUserId")
                        .HasColumnName("CreatedByUserID");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<bool?>("IsPublic")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("((1))");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int?>("PrepTime");

                    b.Property<string>("RecipeUrl")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("ServingSize")
                        .HasMaxLength(50);

                    b.HasKey("RecipeId");

                    b.HasIndex("CreatedByUserId")
                        .HasName("fkIdx_69");

                    b.ToTable("Recipe","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.RecipeIngredient", b =>
                {
                    b.Property<int>("RecipeIngredientId")
                        .HasColumnName("RecipeIngredientID");

                    b.Property<long>("IngredientId")
                        .HasColumnName("IngredientID");

                    b.Property<long>("RecipeId")
                        .HasColumnName("RecipeID");

                    b.Property<string>("Method")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(12, 4)");

                    b.Property<int>("SortOrder");

                    b.Property<string>("UnitOfMeasure")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("RecipeIngredientId", "IngredientId", "RecipeId");

                    b.HasIndex("IngredientId")
                        .HasName("fkIdx_76");

                    b.HasIndex("RecipeId")
                        .HasName("fkIdx_73");

                    b.ToTable("RecipeIngredient","app");
                });

            modelBuilder.Entity("PantryPlanner.Models.RecipeStep", b =>
                {
                    b.Property<int>("RecipeStepId")
                        .HasColumnName("RecipeStepID");

                    b.Property<long>("RecipeId")
                        .HasColumnName("RecipeID");

                    b.Property<int>("SortOrder");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("RecipeStepId", "RecipeId");

                    b.HasIndex("RecipeId")
                        .HasName("fkIdx_87");

                    b.ToTable("RecipeStep","app");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("PantryPlanner.Models.PantryPlannerUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("PantryPlanner.Models.PantryPlannerUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PantryPlanner.Models.PantryPlannerUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("PantryPlanner.Models.PantryPlannerUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.Category", b =>
                {
                    b.HasOne("PantryPlanner.Models.CategoryType", "CategoryType")
                        .WithMany("Category")
                        .HasForeignKey("CategoryTypeId")
                        .HasConstraintName("TypeToCategoryFK")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PantryPlanner.Models.Kitchen", "CreatedByKitchen")
                        .WithMany("Category")
                        .HasForeignKey("CreatedByKitchenId")
                        .HasConstraintName("KitchenToCategoryFK");
                });

            modelBuilder.Entity("PantryPlanner.Models.Ingredient", b =>
                {
                    b.HasOne("PantryPlanner.Models.PantryPlannerUser", "AddedByUser")
                        .WithMany("Ingredient")
                        .HasForeignKey("AddedByUserId")
                        .HasConstraintName("UserToIngredientFK")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PantryPlanner.Models.Category", "Category")
                        .WithMany("Ingredient")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("CategoryToIngredientFK")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("PantryPlanner.Models.IngredientTag", b =>
                {
                    b.HasOne("PantryPlanner.Models.KitchenUser", "CreatedByKitchenUser")
                        .WithMany("IngredientTag")
                        .HasForeignKey("CreatedByKitchenUserId")
                        .HasConstraintName("UserToTagFK");

                    b.HasOne("PantryPlanner.Models.Ingredient", "Ingredient")
                        .WithMany("IngredientTag")
                        .HasForeignKey("IngredientId")
                        .HasConstraintName("IngredientToTagFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PantryPlanner.Models.Kitchen", "Kitchen")
                        .WithMany("IngredientTag")
                        .HasForeignKey("KitchenId")
                        .HasConstraintName("KitchenToTagFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.Kitchen", b =>
                {
                    b.HasOne("PantryPlanner.Models.PantryPlannerUser", "CreatedByUser")
                        .WithMany("Kitchen")
                        .HasForeignKey("CreatedByUserId")
                        .HasConstraintName("UserToKitchenFK")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenIngredient", b =>
                {
                    b.HasOne("PantryPlanner.Models.KitchenUser", "AddedByKitchenUser")
                        .WithMany("KitchenIngredient")
                        .HasForeignKey("AddedByKitchenUserId")
                        .HasConstraintName("KitchenUserToIngredientFK");

                    b.HasOne("PantryPlanner.Models.Category", "Category")
                        .WithMany("KitchenIngredient")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("CategoryToKitchenIngredientFK")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PantryPlanner.Models.Ingredient", "Ingredient")
                        .WithMany("KitchenIngredient")
                        .HasForeignKey("IngredientId")
                        .HasConstraintName("IngredientToKitchenIngredientFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PantryPlanner.Models.Kitchen", "Kitchen")
                        .WithMany("KitchenIngredient")
                        .HasForeignKey("KitchenId")
                        .HasConstraintName("KitchenToKitchenIngredientFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenList", b =>
                {
                    b.HasOne("PantryPlanner.Models.Category", "Category")
                        .WithMany("KitchenList")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("CategoryToListFK")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PantryPlanner.Models.Kitchen", "Kitchen")
                        .WithMany("KitchenList")
                        .HasForeignKey("KitchenId")
                        .HasConstraintName("KitchenToListFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenListIngredient", b =>
                {
                    b.HasOne("PantryPlanner.Models.Recipe", "Recipe")
                        .WithMany("KitchenListIngredient")
                        .HasForeignKey("AddedFromRecipeId")
                        .HasConstraintName("RecipeToListIngredientFK")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PantryPlanner.Models.Ingredient", "Ingredient")
                        .WithMany("KitchenListIngredient")
                        .HasForeignKey("IngredientId")
                        .HasConstraintName("IngredientToListIngredientFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PantryPlanner.Models.KitchenList", "KitchenList")
                        .WithMany("KitchenListIngredient")
                        .HasForeignKey("KitchenListId")
                        .HasConstraintName("KitchenListToIngredientFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenRecipe", b =>
                {
                    b.HasOne("PantryPlanner.Models.Category", "Category")
                        .WithMany("KitchenRecipe")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("CategoryToKitchenRecipeFK")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PantryPlanner.Models.Kitchen", "Kitchen")
                        .WithMany("KitchenRecipe")
                        .HasForeignKey("KitchenId")
                        .HasConstraintName("KitchenToKitchenRecipeFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PantryPlanner.Models.Recipe", "Recipe")
                        .WithMany("KitchenRecipe")
                        .HasForeignKey("RecipeId")
                        .HasConstraintName("RecipeToKitchenRecipeFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.KitchenUser", b =>
                {
                    b.HasOne("PantryPlanner.Models.Kitchen", "Kitchen")
                        .WithMany("KitchenUser")
                        .HasForeignKey("KitchenId")
                        .HasConstraintName("KitchenToKitchenUserFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PantryPlanner.Models.PantryPlannerUser", "User")
                        .WithMany("KitchenUser")
                        .HasForeignKey("UserId")
                        .HasConstraintName("UserToKitchenUserFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.MealPlan", b =>
                {
                    b.HasOne("PantryPlanner.Models.Category", "Category")
                        .WithMany("MealPlan")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("CategoryToMealPlanFK")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PantryPlanner.Models.KitchenUser", "CreatedByKitchenUser")
                        .WithMany("MealPlan")
                        .HasForeignKey("CreatedByKitchenUserId")
                        .HasConstraintName("KitchenUserToMealPlanFK");

                    b.HasOne("PantryPlanner.Models.Kitchen", "Kitchen")
                        .WithMany("MealPlan")
                        .HasForeignKey("KitchenId")
                        .HasConstraintName("KitchenToMealPlanFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.MealPlanRecipe", b =>
                {
                    b.HasOne("PantryPlanner.Models.MealPlan", "MealPlan")
                        .WithMany("MealPlanRecipe")
                        .HasForeignKey("MealPlanId")
                        .HasConstraintName("MealPlanToRecipeFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PantryPlanner.Models.Recipe", "Recipe")
                        .WithMany("MealPlanRecipe")
                        .HasForeignKey("RecipeId")
                        .HasConstraintName("RecipeToMealPlanFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.Recipe", b =>
                {
                    b.HasOne("PantryPlanner.Models.PantryPlannerUser", "CreatedByUser")
                        .WithMany("Recipe")
                        .HasForeignKey("CreatedByUserId")
                        .HasConstraintName("UserToRecipeFK")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("PantryPlanner.Models.RecipeIngredient", b =>
                {
                    b.HasOne("PantryPlanner.Models.Ingredient", "Ingredient")
                        .WithMany("RecipeIngredient")
                        .HasForeignKey("IngredientId")
                        .HasConstraintName("IngredientToRecipeIngredientFK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PantryPlanner.Models.Recipe", "Recipe")
                        .WithMany("RecipeIngredient")
                        .HasForeignKey("RecipeId")
                        .HasConstraintName("RecipeToRecipeIngredientFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PantryPlanner.Models.RecipeStep", b =>
                {
                    b.HasOne("PantryPlanner.Models.Recipe", "Recipe")
                        .WithMany("RecipeStep")
                        .HasForeignKey("RecipeId")
                        .HasConstraintName("RecipeToStepFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
