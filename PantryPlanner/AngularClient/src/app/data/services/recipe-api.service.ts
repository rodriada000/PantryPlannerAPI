import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import Recipe from '../models/Recipe';
import RecipeIngredient from '../models/RecipeIngredient';
import RecipeStep from '../models/RecipeStep';


@Injectable({
  providedIn: 'root'
})
export class RecipeApiService {
  public endPoint = `${environment.baseUrl}/Recipe`;
  public ingredientEndPoint = `${environment.baseUrl}/RecipeIngredient`;
  public stepEndPoint = `${environment.baseUrl}/RecipeStep`;

  constructor(private http: HttpClient) { }

  getRecipesByName(name: string): Observable<Array<Recipe>> {
    return this.http.get<Array<Recipe>>(this.endPoint, {
      params: { 'name': name }
    });
  }

  addRecipe(newRecipe: Recipe): Observable<Recipe> {
    return this.http.post<Recipe>(this.endPoint, newRecipe);
  }

  updateRecipe(updated: Recipe): Observable<any> {
    return this.http.put<any>(this.endPoint + "/" + updated.recipeId.toString(), updated);
  }

  deleteRecipe(toDelete: Recipe): Observable<Recipe> {
    return this.http.delete<Recipe>(this.endPoint + "/" + toDelete.recipeId.toString());
  }


  addRecipeIngredient(toAdd: RecipeIngredient): Observable<RecipeIngredient> {
    return this.http.post<RecipeIngredient>(this.ingredientEndPoint, toAdd);
  }

  updateRecipeIngredient(updated: RecipeIngredient): Observable<any> {
    return this.http.put<any>(this.ingredientEndPoint + "/" + updated.recipeIngredientId.toString(), updated);
  }

  deleteRecipeIngredient(toDelete: RecipeIngredient): Observable<RecipeIngredient> {
    return this.http.delete<RecipeIngredient>(this.ingredientEndPoint + "/" + toDelete.recipeIngredientId.toString());
  }


  addRecipeStep(toAdd: RecipeStep): Observable<RecipeStep> {
    return this.http.post<RecipeStep>(this.stepEndPoint, toAdd);
  }

  updateRecipeStep(updated: RecipeStep): Observable<any> {
    return this.http.put<any>(this.stepEndPoint + "/" + updated.recipeStepId.toString(), updated);
  }

  deleteRecipeStep(toDelete: RecipeStep): Observable<RecipeStep> {
    return this.http.delete<RecipeStep>(this.stepEndPoint + "/" + toDelete.recipeStepId.toString());
  }

}
