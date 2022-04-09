import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

import { environment } from '../../../environments/environment';
import KitchenIngredient from '../models/KitchenIngredient';
import Ingredient from '../models/Ingredient';

@Injectable({
  providedIn: 'root'
})
export default class KitchenIngredientApi {
  public endPoint = `${environment.baseUrl}/KitchenIngredient`;

  private addedIngredient: KitchenIngredient;
  public observableAddedIngredient: BehaviorSubject<KitchenIngredient>;

  constructor(private http: HttpClient) {
    this.addedIngredient = null;
    this.observableAddedIngredient = new BehaviorSubject<KitchenIngredient>(this.addedIngredient);
  }

  public addIngredientToKitchen(ingredient: KitchenIngredient): Observable<KitchenIngredient> {
    return this.http.post<KitchenIngredient>(this.endPoint, ingredient);
  }

  public updateKitchenIngredient(ingredient: KitchenIngredient): Observable<any> {
    return this.http.put<any>(this.endPoint + "/" + ingredient.kitchenIngredientId.toString(), ingredient);
  }

  public removeKitchenIngredient(ingredient: KitchenIngredient): Observable<KitchenIngredient> {
    return this.http.delete<KitchenIngredient>(this.endPoint + "/" + ingredient.kitchenIngredientId.toString());
  }

  public setAddedIngredient(ingredient: KitchenIngredient): void {
    this.addedIngredient = ingredient;
    this.observableAddedIngredient.next(this.addedIngredient);
  }

  public getIngredientsForKitchen(kitchenId: number): Observable<Array<KitchenIngredient>> {
    return this.http.get<Array<KitchenIngredient>>(this.endPoint, {
      params: { 'kitchenId': kitchenId.toString() }
    });
  }

  public createEmpty(i: Ingredient, kitchenId: number): KitchenIngredient {
    const toAdd: KitchenIngredient = new KitchenIngredient();
    toAdd.ingredientId = i.ingredientId;
    toAdd.categoryId = i.categoryId;
    toAdd.kitchenId = kitchenId;
    toAdd.note = "";
    toAdd.quantity = 1;

    return toAdd;
  }
}
