import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import Kitchen from '../models/Kitchen';

@Injectable({
  providedIn: 'root'
})
export default class KitchenApi {
  public API = environment.baseUrl;
  public KITCHEN_ENDPOINT = `${this.API}/Kitchen`;

  constructor(private http: HttpClient) { }

  getAllKitchens(): Observable<Array<Kitchen>> {
    return this.http.get<Array<Kitchen>>(this.KITCHEN_ENDPOINT);
  }

  addKitchen(kitchen: Kitchen): Observable<Kitchen> {
    return this.http.post<Kitchen>(this.KITCHEN_ENDPOINT, kitchen); 
  }

  deleteKitchen(kitchenId: number): Observable<Kitchen> {
    return this.http.delete<Kitchen>(this.KITCHEN_ENDPOINT + "/" + kitchenId.toString());
  }
}
