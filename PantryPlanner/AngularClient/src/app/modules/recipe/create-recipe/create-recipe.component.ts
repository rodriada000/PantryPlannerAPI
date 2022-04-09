import { Component, OnInit } from '@angular/core';
import RecipeIngredient from '../../../data/models/RecipeIngredient';
import RecipeStep from '../../../data/models/RecipeStep';
import { RecipeApiService } from '../../../data/services/recipe-api.service';
import Recipe from '../../../data/models/Recipe';
import { isNullOrUndefined } from 'util';
import { ToastService } from '../../../shared/services/toast.service';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, map, catchError } from 'rxjs/operators';
import Ingredient from '../../../data/models/Ingredient';
import IngredientApi from '../../../data/services/ingredientApi.service';

@Component({
  selector: 'create-recipe',
  templateUrl: './create-recipe.component.html',
  styleUrls: ['./create-recipe.component.css']
})
export class CreateRecipeComponent implements OnInit {

  public recipeUrl: string;
  public name: string;
  public description: string;
  public prepTime: number;
  public cookTime: number;
  public servingSize: string;
  public isPublic: boolean;
  public ingredients: Array<RecipeIngredient>;
  public steps: Array<RecipeStep>;

  public isSaving: boolean;
  private isCreating: boolean;
  private lastSavedRecipe: Recipe;
  public  isSearching: boolean;

  constructor(private recipeApi: RecipeApiService, private ingredientApi: IngredientApi, private toastService: ToastService) {
    this.ingredients = [];
    this.steps = [];
    this.lastSavedRecipe = null;
    this.isPublic = true;
    this.isSaving = false;
  }

  ngOnInit(): void {
  }

  doIngredientSearch = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => this.isSearching = true),
      switchMap(term => term === "" || term.length < 2 ? of([]) :
        this.ingredientApi.getIngredientsByName(term).pipe(
          map(r => r.slice(0, 20)),
          map(r => {
            const createMissing: Ingredient = new Ingredient();
            createMissing.name = "Create Missing Ingredient";
            createMissing.categoryName = "CreateMissing";
            r.push(createMissing);

            return r;
          }),
          catchError(() => {
            return of([]);
          }))
      ),
      tap(() => {
        this.isSearching = false;
      })
    )

  selectIngredient($e: any, index: number): void {
    $e.preventDefault();
    console.log($e.item);
    console.log(index);

    if ($e.item.categoryName === "CreateMissing") {
      return; // todo: show create ingredient modal
    }

    this.ingredients[index].ingredient = $e.item;
    console.log(this.ingredients);
  }

  addEmptyIngredient(): void {
    const empty: RecipeIngredient = new RecipeIngredient();
    empty.recipeIngredientId = 0;
    empty.ingredient = new Ingredient();
    this.ingredients.push(empty);
  }

  addStep(): void {
    const newStep: RecipeStep = new RecipeStep();
    newStep.recipeStepId = 0;
    this.steps.push(newStep);
  }

  saveRecipe(): void {
    this.isSaving = true;

    if (isNullOrUndefined(this.lastSavedRecipe)) {
      this.lastSavedRecipe = new Recipe();
      this.lastSavedRecipe.recipeId = 0;
    }

    this.lastSavedRecipe.ingredients = this.ingredients;
    this.lastSavedRecipe.steps = this.steps;

    this.lastSavedRecipe.recipeUrl = this.recipeUrl;
    this.lastSavedRecipe.name = this.name;
    this.lastSavedRecipe.description = this.description;
    this.lastSavedRecipe.prepTime = this.prepTime;
    this.lastSavedRecipe.cookTime = this.cookTime;
    this.lastSavedRecipe.servingSize = this.servingSize;
    this.lastSavedRecipe.isPublic = this.isPublic;

    if (this.lastSavedRecipe.recipeId === 0) {
      this.recipeApi.addRecipe(this.lastSavedRecipe).subscribe(
        data => {
          console.log(data);
          this.lastSavedRecipe = data;
          this.toastService.showSuccess("Successfully created recipe - " + this.lastSavedRecipe.name);

          // wait till recipe is created before saving back ingredients and steps to get recipe ID
          this.saveIngredients();
          this.saveSteps();


          this.isSaving = false;
        },
        error => {
          this.toastService.showDanger("Failed to create recipe - " + error.error);
          this.isSaving = false;
        }
      );
    } else {
      this.recipeApi.updateRecipe(this.lastSavedRecipe).subscribe(
        data => {
          console.log(data);
          this.toastService.showSuccess("Successfully saved changes.");
          this.isSaving = false;
        },
        error => {
          this.toastService.showDanger("Failed to save recipe changes - " + error.error);
          this.isSaving = false;
        }
      );

      // save ingredients/steps async with saving back recipe since recipe is already created
      this.saveIngredients();
      this.saveSteps();
    }

  }

  saveIngredients(): void {
    if (this.ingredients.length === 0) {
      return; // no ingredients to save back
    }

    if (this.lastSavedRecipe.recipeId === 0) {
      return; // recipe has not been saved/created so return
    }

    for (let i = 0; i < this.ingredients.length; i++) {

      if (isNullOrUndefined(this.ingredients[i].ingredient) || this.ingredients[i].ingredient.ingredientId === 0) {
        return; // skip empty ingredients
      }

      if (this.ingredients[i].recipeIngredientId === 0) {
        // add ingredient to recipe
        this.ingredients[i].recipeId = this.lastSavedRecipe.recipeId;
        this.ingredients[i].ingredientId = this.ingredients[i].ingredient.ingredientId;
        this.ingredients[i].sortOrder = i + 1;

        this.recipeApi.addRecipeIngredient(this.ingredients[i]).subscribe(
          data => {
            this.ingredients[i] = data;
          },
          error => {
            console.error("Could not save ingredient - " + error.error);
          }
        );
      } else {
        // update ingredient
        this.ingredients[i].recipeId = this.lastSavedRecipe.recipeId;
        this.ingredients[i].ingredientId = this.ingredients[i].ingredient.ingredientId;
        this.ingredients[i].sortOrder = i + 1;

        this.recipeApi.updateRecipeIngredient(this.ingredients[i]).subscribe(
          data => {
            console.log("updated ingredient");
          },
          error => {
            console.error("Could not save ingredient - " + error.error);
          }
        );
      }
    }
  }

  saveSteps(): void {
    if (this.steps.length === 0) {
      return; // no steps to save back
    }

    if (this.lastSavedRecipe.recipeId === 0) {
      return; // recipe has not been saved/created so return
    }

    for (let i = 0; i < this.steps.length; i++) {
      if (this.steps[i].recipeStepId === 0) {
        // add step to recipe
        this.steps[i].recipeId = this.lastSavedRecipe.recipeId;
        this.steps[i].sortOrder = i + 1;

        this.recipeApi.addRecipeStep(this.steps[i]).subscribe(
          data => {
            this.steps[i] = data;
          },
          error => {
            console.error("Could not save step - " + error.error);
          }
        );
      } else {
        // update ingredient
        this.steps[i].recipeId = this.lastSavedRecipe.recipeId;
        this.steps[i].sortOrder = i + 1;

        this.recipeApi.updateRecipeStep(this.steps[i]).subscribe(
          data => {
            console.log("updated step");
          },
          error => {
            console.error("Could not save step - " + error.error);
          }
        );
      }
    }
  }

  discardChanges(): void {
    this.recipeUrl = this.lastSavedRecipe.recipeUrl;
    this.name = this.lastSavedRecipe.name;
    this.description = this.lastSavedRecipe.description;
    this.prepTime = this.lastSavedRecipe.prepTime;
    this.cookTime = this.lastSavedRecipe.cookTime;
    this.servingSize = this.lastSavedRecipe.servingSize;
    this.isPublic = this.lastSavedRecipe.isPublic;

    this.ingredients = this.lastSavedRecipe.ingredients;
    this.steps = this.lastSavedRecipe.steps;
  }

}
