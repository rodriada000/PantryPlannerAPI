import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import Ingredient from '../models/Ingredient';
import KitchenIngredient from '../models/KitchenIngredient';
import Kitchen from '../models/Kitchen';

@Injectable({
  providedIn: 'root'
})
export default class IngredientApi {
  public ingredientEndPoint = `${environment.baseUrl}/Ingredient`;
  public kitchenIngredientEndPoint = `${environment.baseUrl}/KitchenIngredient`;


  constructor(private http: HttpClient) { }

  getIngredientsByName(name: string): Observable<Array<Ingredient>> {
    return this.http.get<Array<Ingredient>>(this.ingredientEndPoint, {
      params: { 'name': name }
    });
  }

  addIngredientToKitchen(ingredient: KitchenIngredient): Observable<KitchenIngredient> {
    return this.http.post<KitchenIngredient>(this.kitchenIngredientEndPoint, ingredient); 
  }

  getIngredientsForKitchen(kitchen: Kitchen): Observable<Array<KitchenIngredient>> {
    return this.http.get<Array<KitchenIngredient>>(this.kitchenIngredientEndPoint, {
      params: { 'kitchenId': kitchen.kitchenId.toString() }
    });
  }
}
