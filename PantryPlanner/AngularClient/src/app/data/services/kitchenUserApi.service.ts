import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import KitchenUser from '../models/KitchenUser';

@Injectable({
  providedIn: 'root'
})
export default class KitchenUserApi {
  public endPoint = `${environment.baseUrl}/KitchenUser`;

  constructor(private http: HttpClient) { }

  getAllUsersForKitchen(kitchenId: number): Observable<Array<KitchenUser>> {
    return this.http.get<Array<KitchenUser>>(this.endPoint, {
      params: { 'kitchenId': kitchenId.toString() }
    });
  }

  getKitchenInvitesForLoggedInUser(): Observable<Array<KitchenUser>> {
    return this.http.get<Array<KitchenUser>>(this.endPoint + "/Invite");
  }

  inviteUserToKitchen(username: string, kitchenId: number): Observable<any> {
    return this.http.post<any>(this.endPoint + "/Invite", null, {
      params: {
        'username': username,
        'kitchenId': kitchenId.toString()
      }
    });
  }

  acceptKitchenInvite(kitchenId: number): Observable<any> {
    return this.http.put<any>(this.endPoint + "/Invite", {
      params: {
        'kitchenId': kitchenId.toString()
      }
    });
  }

  denyKitchenInvite(kitchenId: number): Observable<any> {
    return this.http.delete<any>(this.endPoint + "/Invite", {
      params: {
        'kitchenId': kitchenId.toString()
      }
    });
  }

  deleteKitchenUserByKitchenUserId(kitchenUserId: number): Observable<any> {
    return this.http.delete<any>(this.endPoint + "/" + kitchenUserId.toString());
  }

}
