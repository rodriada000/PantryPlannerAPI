using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PantryPlanner.Models;
using System;
using System.Linq;

namespace PantryPlanner.Services
{
    public partial class PantryPlannerContext : IdentityDbContext<PantryPlannerUser>
    {
        public PantryPlannerContext()
        {
        }

        public PantryPlannerContext(DbContextOptions<PantryPlannerContext> options)
            : base(options)
        {
            
        }


        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryType> CategoryType { get; set; }
        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<IngredientTag> IngredientTag { get; set; }
        public virtual DbSet<Kitchen> Kitchen { get; set; }
        public virtual DbSet<KitchenIngredient> KitchenIngredient { get; set; }
        public virtual DbSet<KitchenList> KitchenList { get; set; }
        public virtual DbSet<KitchenListIngredient> KitchenListIngredient { get; set; }
        public virtual DbSet<KitchenRecipe> KitchenRecipe { get; set; }
        public virtual DbSet<KitchenUser> KitchenUser { get; set; }
        public virtual DbSet<MealPlan> MealPlan { get; set; }
        public virtual DbSet<MealPlanRecipe> MealPlanRecipe { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
        public virtual DbSet<RecipeIngredient> RecipeIngredient { get; set; }
        public virtual DbSet<RecipeStep> RecipeStep { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-ADAMR;Initial Catalog=dev_PantryPlanner;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "app");

                entity.HasIndex(e => e.CategoryTypeId)
                    .HasName("fkIdx_161");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryTypeId).HasColumnName("CategoryTypeID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CategoryType)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.CategoryTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("TypeToCategoryFK");

                entity.HasOne(d => d.CreatedByKitchen)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.CreatedByKitchenId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("KitchenToCategoryFK");
            });

            modelBuilder.Entity<CategoryType>(entity =>
            {
                entity.ToTable("CategoryType", "app");

                entity.Property(e => e.CategoryTypeId)
                    .HasColumnName("CategoryTypeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient", "app");

                entity.HasIndex(e => e.AddedByUserId)
                    .HasName("fkIdx_40");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.AddedByUserId).HasColumnName("AddedByUserID");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DateAdded).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.PreviewPicture).HasColumnType("image");

                entity.HasOne(d => d.AddedByUser)
                    .WithMany(p => p.Ingredient)
                    .HasForeignKey(d => d.AddedByUserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("UserToIngredientFK");
            });

            modelBuilder.Entity<IngredientTag>(entity =>
            {
                entity.ToTable("IngredientTag", "app");

                entity.HasIndex(e => e.CreatedByKitchenUserId)
                    .HasName("fkIdx_204");

                entity.HasIndex(e => e.IngredientId)
                    .HasName("fkIdx_198");

                entity.HasIndex(e => e.KitchenId)
                    .HasName("fkIdx_201");

                entity.Property(e => e.IngredientTagId).HasColumnName("IngredientTagID");

                entity.Property(e => e.CreatedByKitchenUserId).HasColumnName("CreatedByKitchenUserID");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.TagName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByKitchenUser)
                    .WithMany(p => p.IngredientTag)
                    .HasForeignKey(d => d.CreatedByKitchenUserId)
                    .HasConstraintName("UserToTagFK");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.IngredientTag)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("IngredientToTagFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.IngredientTag)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToTagFK");
            });

            modelBuilder.Entity<Kitchen>(entity =>
            {
                entity.ToTable("Kitchen", "app");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.CreatedByUserId).HasMaxLength(450);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<KitchenIngredient>(entity =>
            {
                entity.HasKey(e => new { e.KitchenIngredientId, e.IngredientId, e.KitchenId });

                entity.ToTable("KitchenIngredient", "app");

                entity.HasIndex(e => e.AddedByKitchenUserId)
                    .HasName("fkIdx_115");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("fkIdx_187");

                entity.HasIndex(e => e.IngredientId)
                    .HasName("fkIdx_50");

                entity.HasIndex(e => e.KitchenId)
                    .HasName("fkIdx_47");

                entity.Property(e => e.KitchenIngredientId)
                    .HasColumnName("KitchenIngredientID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.AddedByKitchenUserId).HasColumnName("AddedByKitchenUserID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.AddedByKitchenUser)
                    .WithMany(p => p.KitchenIngredient)
                    .HasForeignKey(d => d.AddedByKitchenUserId)
                    .HasConstraintName("KitchenUserToIngredientFK");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KitchenIngredient)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToKitchenIngredientFK");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.KitchenIngredient)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("IngredientToKitchenIngredientFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenIngredient)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToKitchenIngredientFK");
            });

            modelBuilder.Entity<KitchenList>(entity =>
            {
                entity.ToTable("KitchenList", "app");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("fkIdx_175");

                entity.HasIndex(e => e.KitchenId)
                    .HasName("fkIdx_145");

                entity.Property(e => e.KitchenListId).HasColumnName("KitchenListID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KitchenList)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToListFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenList)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToListFK");
            });

            modelBuilder.Entity<KitchenListIngredient>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.KitchenListId, e.IngredientId })
                    .HasName("PK_KitchenListRecipe");

                entity.ToTable("KitchenListIngredient", "app");

                entity.HasIndex(e => e.IngredientId)
                    .HasName("fkIdx_172");

                entity.HasIndex(e => e.KitchenListId)
                    .HasName("fkIdx_178");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.KitchenListId).HasColumnName("KitchenListID");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.KitchenListIngredient)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("IngredientToListIngredientFK");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.KitchenListIngredient)
                    .HasForeignKey(d => d.AddedFromRecipeId)
                    .HasConstraintName("RecipeToListIngredientFK")
                    .OnDelete(DeleteBehavior.SetNull);
                    

                entity.HasOne(d => d.KitchenList)
                    .WithMany(p => p.KitchenListIngredient)
                    .HasForeignKey(d => d.KitchenListId)
                    .HasConstraintName("KitchenListToIngredientFK");
            });

            modelBuilder.Entity<KitchenRecipe>(entity =>
            {
                entity.HasKey(e => new { e.KitchenRecipeId, e.KitchenId, e.RecipeId });

                entity.ToTable("KitchenRecipe", "app");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("fkIdx_190");

                entity.HasIndex(e => e.KitchenId)
                    .HasName("fkIdx_97");

                entity.HasIndex(e => e.RecipeId)
                    .HasName("fkIdx_100");

                entity.Property(e => e.KitchenRecipeId)
                    .HasColumnName("KitchenRecipeID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KitchenRecipe)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToKitchenRecipeFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenRecipe)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToKitchenRecipeFK");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.KitchenRecipe)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("RecipeToKitchenRecipeFK");
            });

            modelBuilder.Entity<KitchenUser>(entity =>
            {
                entity.ToTable("KitchenUser", "app");

                entity.HasIndex(e => e.KitchenId)
                    .HasName("fkIdx_22");

                entity.HasIndex(e => e.UserId)
                    .HasName("fkIdx_19");

                entity.Property(e => e.KitchenUserId).HasColumnName("KitchenUserID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID");

                entity.Property(e => e.HasAcceptedInvite)
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenUser)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToKitchenUserFK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.KitchenUser)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("UserToKitchenUserFK");
            });

            modelBuilder.Entity<MealPlan>(entity =>
            {
                entity.ToTable("MealPlan", "app");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("fkIdx_181");

                entity.HasIndex(e => e.CreatedByKitchenUserId)
                    .HasName("fkIdx_123");

                entity.HasIndex(e => e.KitchenId)
                    .HasName("fkIdx_108");

                entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreatedByKitchenUserId).HasColumnName("CreatedByKitchenUserID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");


                entity.HasOne(d => d.Category)
                    .WithMany(p => p.MealPlan)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToMealPlanFK");

                entity.HasOne(d => d.CreatedByKitchenUser)
                    .WithMany(p => p.MealPlan)
                    .HasForeignKey(d => d.CreatedByKitchenUserId)
                    .HasConstraintName("KitchenUserToMealPlanFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.MealPlan)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToMealPlanFK");
            });

            modelBuilder.Entity<MealPlanRecipe>(entity =>
            {
                entity.HasKey(e => new { e.MealPlanRecipeId, e.RecipeId, e.MealPlanId });

                entity.ToTable("MealPlanRecipe", "app");

                entity.HasIndex(e => e.MealPlanId)
                    .HasName("fkIdx_136");

                entity.HasIndex(e => e.RecipeId)
                    .HasName("fkIdx_133");

                entity.Property(e => e.MealPlanRecipeId).HasColumnName("MealPlanRecipeID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");

                entity.HasOne(d => d.MealPlan)
                    .WithMany(p => p.MealPlanRecipe)
                    .HasForeignKey(d => d.MealPlanId)
                    .HasConstraintName("MealPlanToRecipeFK");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.MealPlanRecipe)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("RecipeToMealPlanFK");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe", "app");

                entity.HasIndex(e => e.CreatedByUserId)
                    .HasName("fkIdx_69");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.IsPublic)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RecipeUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ServingSize).HasMaxLength(50);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Recipe)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("UserToRecipeFK");
            });

            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.HasKey(e => new { e.RecipeIngredientId, e.IngredientId, e.RecipeId });

                entity.ToTable("RecipeIngredient", "app");

                entity.HasIndex(e => e.IngredientId)
                    .HasName("fkIdx_76");

                entity.HasIndex(e => e.RecipeId)
                    .HasName("fkIdx_73");

                entity.Property(e => e.RecipeIngredientId).HasColumnName("RecipeIngredientID");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.Method)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 4)");

                entity.Property(e => e.UnitOfMeasure)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.RecipeIngredient)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("IngredientToRecipeIngredientFK");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeIngredient)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("RecipeToRecipeIngredientFK");
            });

            modelBuilder.Entity<RecipeStep>(entity =>
            {
                entity.HasKey(e => new { e.RecipeStepId, e.RecipeId });

                entity.ToTable("RecipeStep", "app");

                entity.HasIndex(e => e.RecipeId)
                    .HasName("fkIdx_87");

                entity.Property(e => e.RecipeStepId).HasColumnName("RecipeStepID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeStep)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("RecipeToStepFK");
            });
        }

    }
}
