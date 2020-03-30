import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import Kitchen from '../models/Kitchen';

@Injectable()
export default class ApiService {
  public API = 'https://localhost:44317/api';
  public KITCHEN_ENDPOINT = `${this.API}/kitchen`;

  constructor(private http: HttpClient) { }

  getAllKitchens(): Observable<Array<Kitchen>> {
    return this.http.get<Array<Kitchen>>(this.KITCHEN_ENDPOINT);
  }

  addKitchen(kitchen: Kitchen): Observable<Kitchen> {
    return this.http.post<Kitchen>(this.KITCHEN_ENDPOINT, kitchen); 
  }
}
