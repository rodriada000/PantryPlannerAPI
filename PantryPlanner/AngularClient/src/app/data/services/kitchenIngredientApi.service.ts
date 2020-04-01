import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import KitchenIngredient from '../models/KitchenIngredient';
import Kitchen from '../models/Kitchen';

@Injectable({
  providedIn: 'root'
})
export default class KitchenIngredientApi {
  public endPoint = `${environment.baseUrl}/KitchenIngredient`;


  constructor(private http: HttpClient) { }

  addIngredientToKitchen(ingredient: KitchenIngredient): Observable<KitchenIngredient> {
    return this.http.post<KitchenIngredient>(this.endPoint, ingredient); 
  }

  getIngredientsForKitchen(kitchenId: number): Observable<Array<KitchenIngredient>> {
    return this.http.get<Array<KitchenIngredient>>(this.endPoint, {
      params: { 'kitchenId': kitchenId.toString() }
    });
  }
}
