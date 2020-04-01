import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

import { environment } from '../../../environments/environment';
import KitchenIngredient from '../models/KitchenIngredient';

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

  addIngredientToKitchen(ingredient: KitchenIngredient): Observable<KitchenIngredient> {
    return this.http.post<KitchenIngredient>(this.endPoint, ingredient);
  }

  public setAddedIngredient(ingredient: KitchenIngredient): void {
    this.addedIngredient = ingredient;
    this.observableAddedIngredient.next(this.addedIngredient);
  }

  getIngredientsForKitchen(kitchenId: number): Observable<Array<KitchenIngredient>> {
    return this.http.get<Array<KitchenIngredient>>(this.endPoint, {
      params: { 'kitchenId': kitchenId.toString() }
    });
  }
}
