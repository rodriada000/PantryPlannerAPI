import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import Ingredient from '../models/Ingredient';

@Injectable({
  providedIn: 'root'
})
export default class IngredientApi {
  public endPoint = `${environment.baseUrl}/Ingredient`;

  constructor(private http: HttpClient) { }

  getIngredientsByName(name: string): Observable<Array<Ingredient>> {
    return this.http.get<Array<Ingredient>>(this.endPoint, {
      params: { 'name': name }
    });
  }

}
